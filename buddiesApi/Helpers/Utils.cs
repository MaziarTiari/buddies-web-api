using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace buddiesApi.Helpers {
    public static class Utils {
        public static ActionResult UpdateActionResult (UpdateResult updateResult) {
            try {
                if (updateResult.MatchedCount > 0) {
                    return new NoContentResult();
                } else {
                    return new NotFoundResult();
                }
            } catch (Exception) {
                return new ForbidResult();
            }
        }

        public static ActionResult UpdateActionResult(ReplaceOneResult updateResult) {
            try {
                if (updateResult.MatchedCount > 0) {
                    return new NoContentResult();
                } else {
                    return new NotFoundResult();
                }
            } catch (Exception) {
                return new ForbidResult();
            }
        }

        public static long GetCurrentUnixTimestamp() {
            return ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        }
    }
}
