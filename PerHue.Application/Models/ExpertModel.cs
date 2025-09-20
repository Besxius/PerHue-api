using System;

namespace PerHue.Application.Models
{
    public class ExpertModel
    {
        public int Id { get; set; }
        public string? Nickname { get; set; }
        public string Specialization { get; set; }
        public string Bio { get; set; }
        public short YearsOfExperience { get; set; }
        public string? Languages { get; set; }
        public decimal? Rating { get; set; }
        public string Certification { get; set; }
        public string? Introduction { get; set; }
        public string? FacebookAccount { get; set; }
        public string? LinkedInAccount { get; set; }
        public string? InstagramAccount { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
    
    public class UpdateExpertModel
    {
        public string? Nickname { get; set; }
        public string Specialization { get; set; }
        public string Bio { get; set; }
        public short YearsOfExperience { get; set; }
        public string? Languages { get; set; }
        public string Certification { get; set; }
        public string? Introduction { get; set; }
        public string? FacebookAccount { get; set; }
        public string? LinkedInAccount { get; set; }
        public string? InstagramAccount { get; set; }
    }
}