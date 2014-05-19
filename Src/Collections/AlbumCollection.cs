﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK
{
    public class AlbumCollection : BuddyCollectionBase<Album>
    {
        public AlbumCollection()
            : base()
        {
        }

        internal AlbumCollection(BuddyClient client)
            : base(null, client)
        {
        }

        public async Task<BuddyResult<Album>> AddAsync(string name, string caption = null,
            BuddyGeoLocation location = null, string tag = null, BuddyPermissions readPermissions = BuddyPermissions.Default, BuddyPermissions writePermissions = BuddyPermissions.Default)
        {
            var c = new Album(this.Client)
            {
                Name = name,
                Caption = caption,
                Location = location,
                Tag = tag,
                ReadPermissions = readPermissions,
                WritePermissions = writePermissions
            };

            var r = await c.SaveAsync();
            return r.Convert(b => c);
        }

        public Task<SearchResult<Album>> FindAsync(string name = null, string caption = null, string ownerUserId = null,
            BuddyGeoLocationRange locationRange = null, DateRange created = null, DateRange lastModified = null, int pageSize = 100, string pagingToken = null)
        {
            return base.FindAsync(
                userId: ownerUserId,
                created: created,
                lastModified: lastModified,
                locationRange: locationRange,
                pagingToken: pagingToken,
                pageSize: pageSize,
                parameterCallback: (p) =>
                {
                    p["name"] = name;
                    p["caption"] = caption;
                });
        }
    }
}
