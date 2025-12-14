using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.Extensions.Configuration;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.ExpertTestResult;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Services
{
	internal class ExpertTestService : IExpertTestService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public ExpertTestService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_configuration = configuration;
		}

		public async Task<IEnumerable<TestRequestModel>> GetPendingRequestsAsync(int expertId)
		{
			var expertRequests = await _unitOfWork.ExpertTestRequestRepository.GetPendingRequestsForExpertAsync(expertId);

			// Map the underlying TestRequest to TestRequestModel
			var testRequests = expertRequests.Select(etr => etr.TestRequest);
			return _mapper.Map<IEnumerable<TestRequestModel>>(testRequests);
		}
		public async Task<IEnumerable<ExpertAssignmentModel>> GetAllRequestsAsync(int expertId)
		{
			// Fetch the ExpertTestRequests (which contain the Status and Link to TestRequest)
			var expertRequests = await _unitOfWork.ExpertTestRequestRepository.GetAllRequestsForExpertAsync(expertId);

			// Map to the new ExpertAssignmentModel
			var result = expertRequests.Select(etr =>
			{
				// 1. Map the base TestRequest details
				var model = _mapper.Map<ExpertAssignmentModel>(etr.TestRequest);

				// 2. Populate the Expert-specific details
				model.ExpertStatus = etr.Status;           // The status from Expert_TestRequest (Completed/Pending/etc)
				model.AssignmentDate = etr.CreatedDate;    // The date assigned to Expert

				return model;
			});

			return result;
		}

		public async Task<TestResponseModel> SubmitResponseAsync(CreateTestResponseModel model, int expertId)
		{
			// 1. Verify the expert has a pending request
			var pendingRequest = await _unitOfWork.ExpertTestRequestRepository.GetPendingRequestAsync(expertId, model.TestRequestId);
			if (pendingRequest == null)
			{
				throw new InvalidOperationException("No pending test request found for this expert and test.");
			}

			// 2. Save Response
			var testResponse = _mapper.Map<TestResponse>(model);
			testResponse.ExpertId = expertId;
			testResponse.CreatedDate = DateTime.Now;
			testResponse.Type = ResponseTypeEnum.Normal.ToString();

			await _unitOfWork.TestResponseRepository.CreateAsync(testResponse);

			// 3. Update Expert Request Status
			pendingRequest.Status = ExpertTestRequestStatus.Completed.ToString();
			await _unitOfWork.ExpertTestRequestRepository.UpdateAsync(pendingRequest);

			// 4. Save Changes (Expert's action is done)
			await _unitOfWork.SaveChangesWithTransactionAsync();

			// 5. Check for Immediate Finalization
			// Check if we have enough responses to complete the whole test immediately
			var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(model.TestRequestId);
			int requiredResponses = _configuration.GetValue<int>("ExpertTestSettings:RequiredResponses");
			if (requiredResponses == 0) requiredResponses = 3; // Fallback default

			if (responses.Count() >= requiredResponses)
			{
				var testRequest = await _unitOfWork.TestRequestRepository.GetByIdAsync(model.TestRequestId);

				// Only complete if not already completed
				if (testRequest != null && testRequest.Status != TestRequestStatus.Completed.ToString())
				{
					testRequest.Status = TestRequestStatus.Completed.ToString();
					await _unitOfWork.TestRequestRepository.UpdateAsync(testRequest);

					// Send Notification to User
					var notification = new Notification
					{
						Title = "Your Color Analysis is Ready!",
						Content = "Expert Analysis Completed",
						Receiver = testRequest.UserAccountId,
						TestRequestId = testRequest.Id,
						ReceivedTime = DateTime.Now,
						IsRead = false,
						Type = "TestResult"
					};
					await _unitOfWork.NotificationRepository.CreateAsync(notification);

					// Save finalization changes
					await _unitOfWork.SaveChangesWithTransactionAsync();
				}
			}

			return _mapper.Map<TestResponseModel>(testResponse);
		}
		private List<(int R, int G, int B)> ParseHexColors(string colorString)
		{
			if (string.IsNullOrWhiteSpace(colorString))
			{
				return new List<(int R, int G, int B)>();
			}

			// Tách chuỗi theo dấu phẩy và loại bỏ khoảng trắng thừa
			var hexCodes = colorString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
									  .Select(h => h.Trim())
									  .ToList();

			var rgbList = new List<(int R, int G, int B)>();

			foreach (var hex in hexCodes)
			{
				var rgb = ColorCalculationHelper.HexToRgb(hex);
				if (rgb.HasValue)
				{
					rgbList.Add(rgb.Value);
				}
			}
			return rgbList;
		}
		private double CalculateListSimilarity(string listAString, string listBString)
		{
			var rgbA = ParseHexColors(listAString);
			var rgbB = ParseHexColors(listBString);

			// Xử lý trường hợp rỗng
			if (rgbA.Count == 0 && rgbB.Count == 0) return 100.0; // Cả hai rỗng: hoàn toàn giống nhau
			if (rgbA.Count == 0 || rgbB.Count == 0) return 0.0;   // Một rỗng, một không: không giống nhau

			// Đặt danh sách ngắn hơn làm nguồn (source) và danh sách dài hơn làm đích (target)
			var sourceList = rgbA.Count <= rgbB.Count ? rgbA : rgbB;
			var targetList = rgbA.Count <= rgbB.Count ? rgbB : rgbA;

			var unmatchedTargetIndices = Enumerable.Range(0, targetList.Count).ToList();
			double totalSimilarity = 0;

			// Thực hiện Greedy Matching: Ghép mỗi màu trong Source với màu chưa được ghép tốt nhất trong Target
			foreach (var sourceColor in sourceList)
			{
				double maxSimilarity = 0;
				int bestMatchIndex = -1;

				// Tìm màu gần nhất trong danh sách Target chưa được ghép
				foreach (int index in unmatchedTargetIndices)
				{
					var targetColor = targetList[index];
					// Tận dụng hàm đã có: CalculateColorSimilarity (trả về 0-100)
					double similarity = ColorCalculationHelper.CalculateColorSimilarity(sourceColor, targetColor);

					if (similarity > maxSimilarity)
					{
						maxSimilarity = similarity;
						bestMatchIndex = index;
					}
				}

				// Nếu tìm thấy cặp ghép tốt nhất, cộng điểm và đánh dấu là đã ghép
				if (bestMatchIndex != -1)
				{
					totalSimilarity += maxSimilarity;
					unmatchedTargetIndices.Remove(bestMatchIndex);
				}
			}

			// Chuẩn hóa về tỷ lệ 0-100%
			// Chia cho kích thước của danh sách LỚN HƠN (targetList.Count) để phạt nếu có sự thiếu hụt màu
			return totalSimilarity / targetList.Count;
		}
		private double CalculatePairSimilarity(
			TestResponseModel resultA,
			TestResponseModel resultB,
			double seasonWeight = 0.4,
			double bestColorWeight = 0.4,
			double worstColorWeight = 0.2
		)
		{
			// 1. So sánh Màu Mùa (Season Score - S_Season)
			// Giả định ColorTypeId là mã mùa (ví dụ: Light Spring)
			double seasonScore = (resultA.ColorTypeId == resultB.ColorTypeId) ? 1.0 : 0.0;

			// 2. So sánh DS Màu Nên Dùng (Recommended Score - S_Rec)
			double recommendedScore = CalculateListSimilarity(resultA.BestColor, resultB.BestColor) / 100.0;

			// 3. So sánh DS Màu Nên Tránh (Avoided Score - S_Avoid)
			double avoidedScore = CalculateListSimilarity(resultA.WorstColor, resultB.WorstColor) / 100.0;

			// 4. Tính điểm tổng thể (0-100)
			double overallSimilarity =
				(seasonScore * seasonWeight) +
				(recommendedScore * bestColorWeight) +
				(avoidedScore * worstColorWeight);

			return overallSimilarity * 100.0;
		}

		private double? CalculateThreeResultSimilarity(List<TestResponseModel> responses)
		{
			if (responses == null || responses.Count < 3)
			{
				return null;
			}

			var r1 = responses[0];
			var r2 = responses[1];
			var r3 = responses[2];

			double p12 = CalculatePairSimilarity(r1, r2);
			double p13 = CalculatePairSimilarity(r1, r3);
			double p23 = CalculatePairSimilarity(r2, r3);

			return (p12 + p13 + p23) / 3.0;
		}

		public async Task<ExpertTestResultModel> GetExpertResponsesForUserAsync(int testRequestId, int userId)
		{
			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdWithDetailsAsync(testRequestId);

			if (testRequest == null) throw new Exception("Test request not found.");
			if (testRequest.UserAccountId != userId) throw new UnauthorizedAccessException("You are not authorized.");
			//if (testRequest.Status != "Completed") throw new Exception("Test is still being processed.");

			var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequestId);
			var responseModels = _mapper.Map<List<TestResponseModel>>(responses);

			// --- CHECK FOR AI RESULT ---
			// Since GetByIdWithDetailsAsync now Includes AiTestResult, we can use it directly
			if (testRequest.AiTestResult != null)
			{
				var aiResponseModel = new TestResponseModel
				{
					Id = 0,
					TestRequestId = testRequestId,
					ExpertId = 0,
					Type = "AI",
					Note = testRequest.AiTestResult.Note,
					CreatedDate = testRequest.AiTestResult.Date,
					Rating = null,
					BestColor = testRequest.AiTestResult.SuggestedColor,
					WorstColor = testRequest.AiTestResult.AvoidedColor,
					ColorTypeId = testRequest.AiTestResult.ColorTypeId,
					ColorTypeName = testRequest.AiTestResult.ColorType?.Name ?? "Unknown"
				};
				responseModels.Add(aiResponseModel);
			}
			double? similarityScore = null;
			if (responseModels.Count >= 3)
			{
				// Lấy 3 response đầu tiên để tính toán (vì bạn chỉ cần 3 kết quả)
				var top3Responses = responseModels.Take(3).ToList();

				similarityScore = CalculateThreeResultSimilarity(top3Responses);
			}


			return new ExpertTestResultModel
			{
				TestRequest = _mapper.Map<TestRequestModel>(testRequest),
				Responses = responseModels,
				ResponsesSimilarityScore = similarityScore
			};
		}
		public async Task<ExpertTestResultModel> GetExpertResponsesForExpertAsync(int testRequestId, int userId)
		{
			// 1. Get the Test Request details
			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdWithDetailsAsync(testRequestId);

			if (testRequest == null) throw new Exception("Test request not found.");

			// 2. Authorization: Check if this expert is assigned to this request
			if (!await _unitOfWork.TestRequestRepository.IsExpertOfResquest(testRequestId, userId))
				throw new UnauthorizedAccessException("You are not authorized to view this request.");

			// 3. Get responses
			// We fetch responses for the request, then filter for THIS expert's ID only.
			var allResponses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequestId);

			var myResponse = allResponses.Where(r => r.ExpertId == userId).ToList();
			var responseModels = _mapper.Map<List<TestResponseModel>>(myResponse);

			// 4. Return the composite model
			return new ExpertTestResultModel
			{
				TestRequest = _mapper.Map<TestRequestModel>(testRequest),
				Responses = responseModels // Will be [ {response} ] or []
			};
		}
		public async Task<TestResponseModel> UpdateResponseAsync(int testRequestId, UpdateTestResponseModel model, int expertId)
		{
			// 1. [UPDATED] Get the existing response using TestRequestId and ExpertId
			var response = await _unitOfWork.TestResponseRepository.GetByRequestAndExpertAsync(testRequestId, expertId);

			if (response == null)
			{
				throw new Exception("Test response not found for this request and expert.");
			}

			// 2. Security Check: Ensure the expert owns this response 
			// (Technically redundant if fetched by expertId, but good for sanity)
			if (response.ExpertId != expertId)
			{
				throw new UnauthorizedAccessException("You are not authorized to edit this response.");
			}

			// 3. Get the associated TestRequest to check status
			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdAsync(response.TestRequestId);
			if (testRequest == null)
			{
				throw new Exception("Associated test request not found.");
			}

			// 4. Status Check: Only allow editing if NOT "Completed"
			if (testRequest.Status == TestRequestStatus.Completed.ToString())
			{
				throw new InvalidOperationException("Cannot edit response. The test request has already been completed and sent to the user.");
			}

			// 5. Update fields
			response.BestColor = model.BestColor;
			response.WorstColor = model.WorstColor;
			response.ColorTypeId = model.ColorTypeId;
			response.Note = model.Note;

			await _unitOfWork.TestResponseRepository.UpdateAsync(response);
			await _unitOfWork.SaveChangesWithTransactionAsync();

			// 6. Return mapped model
			var colorType = await _unitOfWork.ColorTypeRepository.GetByIdAsync(response.ColorTypeId);
			response.ColorType = colorType;

			return _mapper.Map<TestResponseModel>(response);
		}
		public async Task<IEnumerable<ExpertTestResultModel>> GetAllCompletedExpertTestsAsync()
		{
			var completedTests = await _unitOfWork.TestRequestRepository.GetCompletedExpertTestsAsync();
			var results = new List<ExpertTestResultModel>();

			// This loop can be performance-intensive if you have thousands of tests.
			// For a small to medium number of tests, it is perfectly fine.
			foreach (var test in completedTests)
			{
				var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(test.Id);

				results.Add(new ExpertTestResultModel
				{
					TestRequest = _mapper.Map<TestRequestModel>(test),
					Responses = _mapper.Map<IEnumerable<TestResponseModel>>(responses)
				});
			}

			return results;
		}
		public async Task<IEnumerable<TestRequestModel>> GetAllExpertTestRequestsAsync()
		{
			var requests = await _unitOfWork.TestRequestRepository.GetAllExpertTestsAsync();
			return _mapper.Map<IEnumerable<TestRequestModel>>(requests);
		}

		public async Task<IEnumerable<TestRequestModel>> GetExpertTestRequestsByUserIdAsync(int userId)
		{
			// 1. Use GetByUserIdAsync (Method defined in ITestRequestRepository)
			var requests = await _unitOfWork.TestRequestRepository.GetByUserIdWithDetailsAsync(userId);

			// 2. Filter using "Expert" (Matches the string used in Repository queries)
			var expertRequests = requests.Where(r => r.TypeOfTest == "Expert");

			return _mapper.Map<IEnumerable<TestRequestModel>>(expertRequests);
		}

		public async Task<PaginatedResult<ExpertTestResultModel>> GetMyCompletedExpertTestsAsync(int userId, int pageIndex, int pageSize, DateTime? fromDate, DateTime? toDate)
		{
			// 1. Get Paged Test Requests
			var (testRequests, totalCount) = await _unitOfWork.TestRequestRepository.GetCompletedExpertTestsForUserAsync(userId, pageIndex, pageSize, fromDate, toDate);

			var resultItems = new List<ExpertTestResultModel>();

			// 2. Loop through requests
			foreach (var test in testRequests)
			{
				// Get Expert responses
				var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(test.Id);
				var mappedResponses = _mapper.Map<List<TestResponseModel>>(responses);

				// --- NEW LOGIC: Check for AI Result ---
				if (test.AiTestResult != null)
				{
					var aiResponseModel = new TestResponseModel
					{
						Id = 0, // Indicator for AI
						TestRequestId = test.Id,
						ExpertId = 0, // 0 = AI/System
						Type = "AI",
						Note = test.AiTestResult.Note,
						CreatedDate = test.AiTestResult.Date,
						Rating = null,
						BestColor = test.AiTestResult.SuggestedColor,
						WorstColor = test.AiTestResult.AvoidedColor,
						ColorTypeId = test.AiTestResult.ColorTypeId,
						ColorTypeName = test.AiTestResult.ColorType?.Name ?? "Unknown"
					};
					mappedResponses.Add(aiResponseModel);
				}

				resultItems.Add(new ExpertTestResultModel
				{
					TestRequest = _mapper.Map<TestRequestModel>(test),
					Responses = mappedResponses
				});
			}

			// 3. Return Paginated Result
			return new PaginatedResult<ExpertTestResultModel>
			{
				Items = resultItems,
				TotalCount = totalCount,
				PageIndex = pageIndex,
				PageSize = pageSize,
				TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
			};
		}
		/*public async Task RateExpertResponseAsync(RateExpertResponseModel model, int userId)
		{
			// 1. Get the TestResponse and its parent TestRequest
			var testResponse = await _unitOfWork.TestResponseRepository.GetByIdAsync(model.TestResponseId);
			if (testResponse == null)
			{
				throw new Exception("Test response not found.");
			}

			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdAsync(testResponse.TestRequestId);
			if (testRequest == null)
			{
				throw new Exception("Parent test request not found.");
			}

			// 2. Security Check: Ensure the user rating this is the one who created the test
			if (testRequest.UserAccountId != userId)
			{
				throw new UnauthorizedAccessException("You are not authorized to rate this response.");
			}

			// 3. Check if already rated
			if (testResponse.Rating != null)
			{
				throw new InvalidOperationException("This response has already been rated.");
			}

			// 4. Save the rating to the TestResponse
			testResponse.Rating = model.Rating;
			await _unitOfWork.TestResponseRepository.UpdateAsync(testResponse);

			// 5. Recalculate the Expert's average rating
			var expert = await _unitOfWork.ExpertRepository.GetByIdAsync(testResponse.ExpertId);
			if (expert == null)
			{
				// This shouldn't happen, but good to check
				throw new Exception("Expert not found.");
			}

			// Get all *rated* responses for this expert
			var allRatedResponses = await _unitOfWork.TestResponseRepository.GetAllByExpertIdAsync(expert.Id);

			// Calculate new average
			var newAverage = allRatedResponses.Average(r => r.Rating);
			expert.Rating = (decimal)newAverage;

			await _unitOfWork.ExpertRepository.UpdateAsync(expert);

			// 6. Save all changes in one transaction
			await _unitOfWork.SaveChangesWithTransactionAsync();
		}*/
		public async Task RateExpertResponseAsync(RateExpertResponseModel model, int userId)
		{
			// 1. Get the TestResponse and its parent TestRequest
			var testResponse = await _unitOfWork.TestResponseRepository.GetByIdAsync(model.TestResponseId);
			if (testResponse == null) throw new Exception("Test response not found.");

			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdAsync(testResponse.TestRequestId);
			if (testRequest == null) throw new Exception("Parent test request not found.");

			// 2. Security Check
			if (testRequest.UserAccountId != userId) throw new UnauthorizedAccessException("You are not authorized to rate this response.");

			// 3. Check if already rated
			if (testResponse.Rating != null) throw new InvalidOperationException("This response has already been rated.");

			// 4. Save the rating to the TestResponse
			testResponse.Rating = model.Rating;
			await _unitOfWork.TestResponseRepository.UpdateAsync(testResponse);

			// 5. Recalculate the Expert's Overall Rating
			var expert = await _unitOfWork.ExpertRepository.GetByIdAsync(testResponse.ExpertId);
			if (expert == null) throw new Exception("Expert not found.");

			// A. Calculate Average from "Rated" Responses
			var allResponses = await _unitOfWork.TestResponseRepository.GetAllByExpertIdAsync(expert.Id);
			var ratedResponses = allResponses.Where(r => r.Rating != null && r.Rating > 0).ToList();

			decimal averageRating = 0;
			if (ratedResponses.Any())
			{
				averageRating = (decimal)ratedResponses.Average(r => r.Rating!.Value);
			}
			else
			{
				averageRating = 5.0m;
			}

			// B. Calculate Total Penalty from "Expired" Requests
			var expiredRequests = await _unitOfWork.ExpertTestRequestRepository
				.FindAsync(r => r.ExpertId == expert.Id && r.Status == "Expired");

			int expiredCount = expiredRequests.Count();

			// Get penalty amount from configuration (default 0.2)
			decimal ratingDeduction = _configuration.GetValue<decimal>("ExpertTestSettings:RatingDeduction");
			decimal totalPenalty = expiredCount * ratingDeduction;

			// C. Apply Formula: Rating = Average - Penalty
			decimal finalRating = averageRating - totalPenalty;

			// D. Ensure Rating doesn't drop below 0
			if (finalRating < 0) finalRating = 0;

			expert.Rating = finalRating;
			await _unitOfWork.ExpertRepository.UpdateAsync(expert);

			// 6. Save all changes in one transaction
			await _unitOfWork.SaveChangesWithTransactionAsync();
		}
		public async Task<IEnumerable<ReviewTestRequestModel>> GetPendingReviewRequestsAsync(int expertId)
		{
			// 1. Fetch requests with "PendingReview" status
			var expertRequests = await _unitOfWork.ExpertTestRequestRepository.GetPendingReviewRequestsForExpertAsync(expertId);

			var result = new List<ReviewTestRequestModel>();

			foreach (var req in expertRequests)
			{
				// 2. Map the TestRequest
				var requestModel = _mapper.Map<TestRequestModel>(req.TestRequest);

				// 3. Map the Responses (The 3 initial ones)
				// Note: We filter out any potential 'Review' type responses to be safe, showing only the original work.
				var previousResponses = req.TestRequest.TestResponses
					.Where(r => r.Type != ResponseTypeEnum.Review.ToString())
					.ToList();

				var responseModels = _mapper.Map<IEnumerable<TestResponseModel>>(previousResponses);

				result.Add(new ReviewTestRequestModel
				{
					ExpertTestRequestId = req.ExpertId, // Or the composite ID if needed, usually TestRequestId is enough context
					TestRequest = requestModel,
					PreviousResponses = responseModels
				});
			}

			return result;
		}
		public async Task<ReviewTestRequestModel> GetPendingReviewRequestsByIdAsync(int expertId, int testRequestId)
		{
			// 1. Fetch requests with "PendingReview" status
			var expertRequests = await _unitOfWork.ExpertTestRequestRepository.GetPendingReviewRequestsForExpertAsync(expertId);

			var reviewRequest = expertRequests.FirstOrDefault(p => p.TestRequest.Id == testRequestId);

			// 2. Map the TestRequest
			var requestModel = _mapper.Map<TestRequestModel>(reviewRequest.TestRequest);

			// 3. Map the Responses (The 3 initial ones)
			// Note: We filter out any potential 'Review' type responses to be safe, showing only the original work.
			var previousResponses = reviewRequest.TestRequest.TestResponses
				.Where(r => r.Type != ResponseTypeEnum.Review.ToString())
				.ToList();

			var responseModels = _mapper.Map<IEnumerable<TestResponseModel>>(previousResponses);

			var result = new ReviewTestRequestModel
			{
				ExpertTestRequestId = reviewRequest.ExpertId,
				TestRequest = requestModel,
				PreviousResponses = responseModels
			};

			return result;
		}

		// --- IMPLEMENT VOTING ---
		public async Task<TestResponseModel> VoteForResponseAsync(VoteResponseModel model, int expertId)
		{
			// 1. Verify the expert has a pending request for this test
			var pendingRequest = await _unitOfWork.ExpertTestRequestRepository.GetPendingReviewRequestAsync(expertId, model.TestRequestId);

			if (pendingRequest == null)
			{
				throw new InvalidOperationException("No pending review request found for this expert.");
			}

			// 2. Get the response the expert voted for
			var votedResponse = await _unitOfWork.TestResponseRepository.GetByIdAsync(model.VotedResponseId);
			if (votedResponse == null || votedResponse.TestRequestId != model.TestRequestId)
			{
				throw new ArgumentException("Invalid response selected.");
			}

			// 3. Create the Review Response (Copying data)
			var reviewResponse = new TestResponse
			{
				TestRequestId = model.TestRequestId,
				ExpertId = expertId,
				CreatedDate = DateTime.Now,
				Type = ResponseTypeEnum.Review.ToString(), // Set Type to Review

				// Copy core analysis data
				BestColor = votedResponse.BestColor,
				WorstColor = votedResponse.WorstColor,
				ColorTypeId = votedResponse.ColorTypeId,

				// Add note indicating it's a review/vote
				Note = string.IsNullOrWhiteSpace(model.Note)
					? $"Reviewed and agreed with Expert {votedResponse.ExpertId}."
					: model.Note
			};

			await _unitOfWork.TestResponseRepository.CreateAsync(reviewResponse);

			// 4. Mark request as completed
			pendingRequest.Status = ExpertTestRequestStatus.Completed.ToString();
			await _unitOfWork.ExpertTestRequestRepository.UpdateAsync(pendingRequest);

			// --- STATUS CHANGE: Mark main request back to Completed ---
			var mainRequest = await _unitOfWork.TestRequestRepository.GetByIdAsync(model.TestRequestId);
			if (mainRequest != null)
			{
				mainRequest.Status = TestRequestStatus.Completed.ToString();
				await _unitOfWork.TestRequestRepository.UpdateAsync(mainRequest);

				// --- NOTIFICATION: For the User ---
				var notification = new Notification
				{
					Title = "Review Completed",
					Content = "The expert review you requested has been completed.",
					Receiver = mainRequest.UserAccountId,
					TestRequestId = model.TestRequestId,
					ReceivedTime = DateTime.Now,
					IsRead = false,
					Type = "ReviewResult"
				};
				await _unitOfWork.NotificationRepository.CreateAsync(notification);
			}

			await _unitOfWork.SaveChangesWithTransactionAsync();

			// 5. Return mapped model (Fetching color type name handled by mapper if entity loaded, or we rely on lazy loading/repo include)
			// To be safe, let's load the ColorType for mapping
			var colorType = await _unitOfWork.ColorTypeRepository.GetByIdAsync(reviewResponse.ColorTypeId);
			reviewResponse.ColorType = colorType;

			return _mapper.Map<TestResponseModel>(reviewResponse);
		}
	}
}


/*public async Task<ExpertTestResultModel> GetExpertResponsesForUserAsync(int testRequestId, int userId)
		{
			// 1. Get the main test request, now with all details
			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdWithDetailsAsync(testRequestId);

			if (testRequest == null)
			{
				throw new Exception("Test request not found.");
			}

			// 2. Security Check: Ensure the user asking for the test is the one who created it
			if (testRequest.UserAccountId != userId)
			{
				throw new UnauthorizedAccessException("You are not authorized to view this test result.");
			}

			// 3. Check if the test is finished
			if (testRequest.Status != "Completed")
			{
				throw new Exception("Your test is still being processed by our experts.");
			}

			// 4. Get the individual responses
			var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequestId);

			// 5. Build the complete response DTO
			var result = new ExpertTestResultModel
			{
				TestRequest = _mapper.Map<TestRequestModel>(testRequest),
				Responses = _mapper.Map<IEnumerable<TestResponseModel>>(responses)
			};

			return result;
		}*/
/*public async Task<ExpertTestResultModel> GetExpertResponsesForUserAsync(int testRequestId, int userId)
{
	// 1. Get the main test request
	var testRequest = await _unitOfWork.TestRequestRepository.GetByIdWithDetailsAsync(testRequestId);

	if (testRequest == null)
	{
		throw new Exception("Test request not found.");
	}

	// 2. Security Check
	if (testRequest.UserAccountId != userId)
	{
		throw new UnauthorizedAccessException("You are not authorized to view this test result.");
	}

	// 3. Check status
	if (testRequest.Status != "Completed")
	{
		throw new Exception("Your test is still being processed.");
	}

	// 4. Get EXPERT responses
	var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequestId);
	var responseModels = _mapper.Map<List<TestResponseModel>>(responses);

	// 5. CHECK FOR AI RESULT (The Fallback)
	// Assuming specific repository method exists or using FindAsync
	// Since AiTestResult ID is Shared PK with TestRequest:
	var aiResult = await _unitOfWork.AiTestResultRepository.GetByIdAsync(testRequestId);

	if (aiResult != null)
	{
		// Map AI Result to a "Fake" TestResponseModel so it fits in the list
		var aiResponseModel = new TestResponseModel
		{
			Id = 0, // Or some indicator that it's AI
			TestRequestId = testRequestId,
			ExpertId = 0, // 0 indicates System/AI
						  // You might want to fetch "AI" expert Name in frontend or hardcode logic here
						  // Note: ExpertName is not in TestResponseModel currently, but if you added it:
						  // ExpertName = "PerHue AI Assistant", 

			Note = aiResult.Note,
			CreatedDate = aiResult.Date,
			Rating = null, // User can't rate AI? Or maybe they can.
			BestColor = aiResult.SuggestedColor,
			WorstColor = aiResult.AvoidedColor,
			ColorTypeId = aiResult.ColorTypeId,
			ColorTypeName = aiResult.ColorType?.Name ?? "Unknown" // Ensure ColorType is included in repo query
		};

		// If ColorType was null in aiResult, fetch name manually
		if (aiResult.ColorType == null && aiResult.ColorTypeId > 0)
		{
			var ct = await _unitOfWork.ColorTypeRepository.GetByIdAsync(aiResult.ColorTypeId);
			aiResponseModel.ColorTypeName = ct?.Name ?? "Unknown";
		}

		responseModels.Add(aiResponseModel);
	}

	// 6. Build Final Result
	var result = new ExpertTestResultModel
	{
		TestRequest = _mapper.Map<TestRequestModel>(testRequest),
		Responses = responseModels
	};

	return result;
}*/