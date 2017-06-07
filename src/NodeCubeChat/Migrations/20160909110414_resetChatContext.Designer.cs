using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NodeCubeChat.Models;

namespace NodeCubeChat.Migrations
{
    [DbContext(typeof(ChatContext))]
    [Migration("20160909110414_resetChatContext")]
    partial class resetChatContext
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NodeCubeChat.Models.Ad", b =>
                {
                    b.Property<int>("AdId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdPackageId");

                    b.Property<int>("CompanyId");

                    b.Property<int>("Displays");

                    b.HasKey("AdId");

                    b.ToTable("Ads");
                });

            modelBuilder.Entity("NodeCubeChat.Models.AdPackage", b =>
                {
                    b.Property<int>("AdPackageId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdPackageName");

                    b.Property<double>("AdPackagePrice");

                    b.HasKey("AdPackageId");

                    b.ToTable("AdPackages");
                });

            modelBuilder.Entity("NodeCubeChat.Models.ChatRoom", b =>
                {
                    b.Property<int>("ChatRoomId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RoomName");

                    b.HasKey("ChatRoomId");

                    b.ToTable("ChatRooms");
                });

            modelBuilder.Entity("NodeCubeChat.Models.Company", b =>
                {
                    b.Property<int>("CompanyId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdPackageIds");

                    b.Property<int>("Displays");

                    b.Property<string>("Name");

                    b.Property<bool>("Paid");

                    b.HasKey("CompanyId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("NodeCubeChat.Models.Conversation", b =>
                {
                    b.Property<int>("ConversationId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Accepted");

                    b.Property<DateTime?>("DateAccepted");

                    b.Property<DateTime?>("DateInitiated");

                    b.Property<int>("InitiatorId");

                    b.Property<int>("MessagesSent");

                    b.Property<int>("PartnerId");

                    b.HasKey("ConversationId");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("NodeCubeChat.Models.GlobalMessage", b =>
                {
                    b.Property<int>("GlobalMessageId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Message");

                    b.Property<string>("RoomName");

                    b.Property<int>("SenderId");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("GlobalMessageId");

                    b.ToTable("GlobalMessages");
                });

            modelBuilder.Entity("NodeCubeChat.Models.Image", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Extension");

                    b.Property<string>("FileName");

                    b.Property<string>("OwnerUsername");

                    b.Property<bool>("Private");

                    b.Property<string>("ServerPath");

                    b.HasKey("ImageId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("NodeCubeChat.Models.ImageRating", b =>
                {
                    b.Property<int>("ImageRatingId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ImageId");

                    b.Property<string>("RatingUser");

                    b.Property<int>("Score");

                    b.HasKey("ImageRatingId");

                    b.ToTable("ImageRatings");
                });

            modelBuilder.Entity("NodeCubeChat.Models.PrivateMessage", b =>
                {
                    b.Property<int>("PrivateMessageId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ConversationId");

                    b.Property<string>("Message");

                    b.Property<int>("SenderId");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("PrivateMessageId");

                    b.ToTable("PrivateMessages");
                });

            modelBuilder.Entity("NodeCubeChat.Models.Survey", b =>
                {
                    b.Property<int>("SurveyId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Description");

                    b.Property<short>("RequiredGender");

                    b.Property<int>("RequiredMaxAge");

                    b.Property<int>("RequiredMinAge");

                    b.Property<bool>("RequiredPositiveUserRating");

                    b.Property<string>("Title");

                    b.HasKey("SurveyId");

                    b.ToTable("Surveys");
                });

            modelBuilder.Entity("NodeCubeChat.Models.SurveyOption", b =>
                {
                    b.Property<int>("SurveyOptionId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AttachedTextbox");

                    b.Property<int>("SurveyQuestionId");

                    b.Property<string>("Title");

                    b.HasKey("SurveyOptionId");

                    b.ToTable("SurveyOptions");
                });

            modelBuilder.Entity("NodeCubeChat.Models.SurveyQuestion", b =>
                {
                    b.Property<int>("SurveyQuestionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<int>("SurveyId");

                    b.Property<string>("Title");

                    b.Property<string>("Type");

                    b.HasKey("SurveyQuestionId");

                    b.ToTable("SurveyQuestions");
                });

            modelBuilder.Entity("NodeCubeChat.Models.SurveyStatistic", b =>
                {
                    b.Property<int>("SurveyStatisticId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CoolDownPeriod");

                    b.Property<bool>("FinishedSurvey");

                    b.Property<string>("ParticipantUserName");

                    b.Property<int>("SurveyId");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("SurveyStatisticId");

                    b.ToTable("SurveyStatistics");
                });

            modelBuilder.Entity("NodeCubeChat.Models.SurveyUserAnswer", b =>
                {
                    b.Property<int>("SurveyUserAnswerId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("SurveyId");

                    b.Property<int>("SurveyOptionId");

                    b.Property<int>("SurveyQuestionId");

                    b.Property<string>("SurveyText");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("SurveyUserAnswerId");

                    b.ToTable("SurveyUserAnswers");
                });

            modelBuilder.Entity("NodeCubeChat.Models.UserRating", b =>
                {
                    b.Property<int>("UserRatingId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RatedUser");

                    b.Property<string>("RatingUser");

                    b.Property<bool>("Score");

                    b.HasKey("UserRatingId");

                    b.ToTable("UserRatings");
                });
        }
    }
}
