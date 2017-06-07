using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NodeCubeChat.Models
{
    public class ChatContext : DbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options)
            : base(options)
        { }

        public DbSet<GlobalMessage> GlobalMessages { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<AdPackage> AdPackages { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ImageRating> ImageRatings { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyStatistic> SurveyStatistics { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<SurveyOption> SurveyOptions { get; set; }
        public DbSet<SurveyUserAnswer> SurveyUserAnswers { get; set; }
    }

    public class GlobalMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GlobalMessageId { get; set; }
        public DateTime TimeStamp { get; set; }
        public int SenderId { get; set; }
        public string Message { get; set; }
        public string RoomName { get; set; }
    }

    public class ChatRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChatRoomId { get; set; }
        public string RoomName { get; set; }
    }

    public class PrivateMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PrivateMessageId { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
    }

    public class Conversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConversationId { get; set; }
        public int InitiatorId { get; set; }
        public int PartnerId { get; set; }
        public int MessagesSent { get; set; }
        public DateTime? DateInitiated { get; set; }
        public bool Accepted { get; set; }
        public DateTime? DateAccepted { get; set; }
    }

    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string AdPackageIds { get; set; }
        public int Displays { get; set; }
        public bool Paid { get; set; }
    }

    public class Ad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdId { get; set; }
        public int CompanyId { get; set; }
        public int AdPackageId { get; set; }
        public int Displays { get; set; }
    }
    public class AdPackage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdPackageId { get; set; }
        public string AdPackageName { get; set; }
        public double AdPackagePrice { get; set; }
    }

    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }
        public string OwnerUsername { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string ServerPath { get; set; }
        public bool Private { get; set; }
    }
    public class ImageRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageRatingId { get; set; }
        public string RatingUser { get; set; }
        public int ImageId { get; set; }
        public int Score  { get; set; }
    }
    public class UserRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserRatingId { get; set; }
        public string RatingUser { get; set; }
        public string RatedUser { get; set; }
        public bool Score { get; set; }
    }

    public class Survey
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SurveyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public short RequiredGender { get; set; } //0 - All genders, 1 - Male only, 2 - Female only
        public int RequiredMinAge { get; set; }
        public int RequiredMaxAge { get; set; }
        public bool RequiredPositiveUserRating { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Active { get; set; }
    }
    
    public class SurveyStatistic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SurveyStatisticId { get; set; }
        public int SurveyId { get; set; }
        public string ParticipantUserName { get; set; }
        public bool FinishedSurvey { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime CoolDownPeriod { get; set; }
    }

    public class SurveyQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SurveyQuestionId { get; set; }
        public int SurveyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // "Radio" - Radio button, "Text" - Free form input, "Select" - dropdown menu, "Checkbox" - ... checkbox.
    }

    public class SurveyOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SurveyOptionId { get; set; }
        public int SurveyQuestionId { get; set;}
        public string Title { get; set; }
        public bool AttachedTextbox { get; set; }
    }

    public class SurveyUserAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SurveyUserAnswerId { get; set; }
        public int SurveyId { get; set; }
        public int SurveyQuestionId { get; set; }
        public int SurveyOptionId { get; set; }
        public string SurveyText { get; set; }
        public DateTime Timestamp {get;set;}
    }

}
