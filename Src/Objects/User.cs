using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BuddySDK
{
    /// <summary>
    /// Represents the gender of a user.
    /// </summary>
    public enum UserGender
    {
		Unknown,
		Male,
        Female
    }

  

    /// <summary>
    /// Represents a public user profile. Public user profiles are usually returned when looking at an AuthenticatedUser's friends or making a search with FindUser.
    /// <example>
    /// <code>
    ///    
    ///     var loggedInUser = await Buddy.LoginUserAsync("username", "password");
    /// </code>
    /// </example>
    /// </summary>
    /// 
    [BuddyObjectPath("/users")]
    public class User : BuddyBase
    {

		[JsonProperty("firstName")]
		public string FirstName
        {
            get
            {
				return GetValueOrDefault<string>("FirstName");
            }
            set
            {
				SetValue("FirstName", value);
            }
        }

		[JsonProperty("lastName")]
		public string LastName
		{
			get
			{
				return GetValueOrDefault<string>("LastName");
			}
			set
			{
				SetValue("LastName", value);
			}
		}    
    
        [JsonProperty("userName")]
        public string Username
        {
            get
            {
                return GetValueOrDefault<string>("Username");
            }
            set
            {
                SetValue<string>("Username", value, checkIsProp: false);
            }
            
        }

        [JsonProperty("email")]
        public string Email
        {
            get
            {
                return GetValueOrDefault<string>("Email");
            }
            set
            {
                SetValue<string>("Email", value, checkIsProp: false);
            }

        }
        /// <summary>
        /// Gets the gender of the user.
        /// </summary>
        [JsonProperty("gender")]
        public UserGender? Gender
        {
            get
            {
                return GetValueOrDefault<UserGender?>("Gender");
            }
            set
            {
                SetValue<UserGender?>("Gender", value, checkIsProp: false);
            }
        }

        [JsonProperty("dateOfBirth")]
        public DateTime? DateOfBirth
        {
            get
            {
                return GetValueOrDefault<DateTime?>("DateOfBirth");
            }
            set
            {
                SetValue<DateTime?>("DateOfBirth", value, checkIsProp: false);
            }
        }
      
        /// <summary>
        /// Gets the age of this user.
        /// </summary>
        public int? Age
        {
            get
            {
                var dob = this.DateOfBirth;

                if (dob != null)
                {
                    return (int)(DateTime.Now.Subtract (dob.Value).TotalDays / 365.25);
                }

                return null;
            }
        }

        [JsonProperty("profilePictureID")]
        public string ProfilePictureID
        {
            get
            {
                return GetValueOrDefault<string>("ProfilePictureID");
            }
            set
            {
                ProfilePicture = value == null ? null : new Picture(value);
            }
        }

        [JsonProperty("profilePictureUrl")]
        public string ProfilePictureUrl
        {
            get
            {
                return GetValueOrDefault<string>("ProfilePictureUrl");
            }
        }

        private Picture profilePicture;
        public Picture ProfilePicture
        {
            get
            {
                if (profilePicture == null || profilePicture.ID != ProfilePictureID || profilePicture.SignedUrl != ProfilePictureUrl)
                {
                    ProfilePicture = new Picture(ProfilePictureID, ProfilePictureUrl);
                }

                return profilePicture;
            }
            set
            {
                profilePicture = value;

                SetValue<string>("ProfilePictureID", value == null ? null : value.ID, checkIsProp: false);
                SetValue<string>("ProfilePictureUrl", value == null ? null : value.SignedUrl, checkIsProp: false);
            }
        }

        internal User(BuddyClient client = null): base(client)
        {
        }

        public User(string id, BuddyClient client = null)
            : base(id, client)
        {
        }

        public async Task<BuddyResult<Picture>> AddProfilePictureAsync(string caption, Stream pictureData, string contentType, BuddyGeoLocation location = null,
            BuddyPermissions readPermissions = BuddyPermissions.Default, BuddyPermissions writePermissions = BuddyPermissions.Default)
        {
           var result = await PictureCollection.AddAsync(this.Client, caption, pictureData, contentType, location,
               readPermissions, writePermissions);

           if (result.IsSuccess)
           {
               ProfilePicture = result.Value;
           }

           return result;
        }

        public override async Task<BuddyResult<bool>> FetchAsync(Action updateComplete = null)
        {

            var r = await base.FetchAsync(updateComplete);


            if (r.IsSuccess) {

                if (!string.IsNullOrEmpty(ProfilePictureID))
                {
                    await ProfilePicture.FetchAsync();
                }
            }
                  
            return r;
        }

        public override async Task<BuddyResult<bool>> SaveAsync()
        {
            Username = Username; // TODO: user name is required on PATCH, so do this to ensure it gets added to the PATCH dictionary.  Remove when user name is optional

            return await Task.Run<BuddyResult<bool>> (async () => {

                var baseResult = await base.SaveAsync();

                if (ProfilePicture != null)
                {
                    var pictureResult = await ProfilePicture.SaveAsync();

                    if (!pictureResult.IsSuccess)
                    {
                        return pictureResult;
                    }
                }

                return baseResult;
            });
        }

       
    }
}