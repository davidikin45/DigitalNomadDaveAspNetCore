using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Solution.Base.Helpers;
using Solution.Base.Extensions;

namespace Solution.Base.APIs
{
    public class Instagram
    {
        public class User
        {
            public async static Task<Dictionary<string, string>> GetUserPostById(string clientID, string userID, string id)
            {
                var posts = await Instagram.User.GetUserLatestPosts(clientID, userID);
                foreach (Dictionary<string, string> post in posts)
                {
                    if (post["id"] == id)
                    {
                        return post;
                    }
                }
                return null;
            }

            public async static Task<List<Dictionary<string, string>>> GetUserLatestPosts(string accessToken, string userID)
            {
                ArrayList result = await Instagram.User.QueryUser(accessToken, userID, "");
                return Instagram.User.GetUserLatestPostsFromResult(result);
            }

            public async static Task<ArrayList> QueryUser(string accessToken, string userID, string url = "")
            {
                string jsonString;
                if (url == "")
                {
                    var response = await WebRequestHelper.GetAsync(string.Format("https://api.instagram.com/v1/users/{0}/media/recent/?access_token={1}", userID, accessToken), WebRequestHelper.ResponseAcceptType.JSON, TaskHelper.CreateLinkedCancellationToken());
                    jsonString = response.Item1.BytesToString();
                }
                else
                {
                    var response = await WebRequestHelper.GetAsync(string.Format(url, new object[0]), WebRequestHelper.ResponseAcceptType.JSON, TaskHelper.CreateLinkedCancellationToken());
                    jsonString = response.Item1.BytesToString();
                }

                Dictionary<string, object> dictionary = jsonString.DeserializeJsonToDictionary();
                ArrayList arrayList = (ArrayList)dictionary["data"];
                if (dictionary.ContainsKey("pagination"))
                {
                    Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["pagination"];
                    if (dictionary2.ContainsKey("next_url"))
                    {
                        string url2 = (string)dictionary2["next_url"];
                        arrayList.AddRange(await Instagram.User.QueryUser("", "", url2));
                    }
                }
                return arrayList;
            }

            public async static Task<string> GetUserId(string accessToken, string userName)
            {
                var result = await Instagram.User.GetUser(accessToken, userName);
                Dictionary<string, object> user = result;
                if (user != null && user.ContainsKey("id"))
                {
                    return (string)user["id"];
                }
                return "";
            }

            public async static Task<string> GetUserProfilePicture(string accessToken, string userName)
            {
                var result = await Instagram.User.GetUser(accessToken, userName);
                Dictionary<string, object> user = result;
                if (user != null && user.ContainsKey("profile_picture"))
                {
                    return (string)user["profile_picture"];
                }
                return "";
            }

            //231022169.60d8eef.59c2b28825cc40f490c7ff69b9d170e9

            public async static Task<Dictionary<string, object>> GetUser(string accessToken, string userName)
            {
                var result = await Instagram.User.QueryUserId(accessToken, userName);
                return (Dictionary<string, object>)result[0];
            }

            public async static Task<ArrayList> QueryUserId(string accessToken, string userName)
            {
                var response = await WebRequestHelper.GetAsync(string.Format("https://api.instagram.com/v1/users/search?q={0}&access_token={1}", userName, accessToken), WebRequestHelper.ResponseAcceptType.JSON, TaskHelper.CreateLinkedCancellationToken());
                var jsonString = response.Item1.BytesToString();
                Dictionary<string, object> dictionary = jsonString.DeserializeJsonToDictionary();
                return (ArrayList)dictionary["data"];
            }

            private static List<Dictionary<string, string>> GetUserLatestPostsFromResult(ArrayList result)
            {
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

                foreach (Dictionary<string, object> dict in result)
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    dictionary.Add("id", "");
                    dictionary.Add("link", "");
                    dictionary.Add("thumbnail-image", "");
                    dictionary.Add("lowres-image", "");
                    dictionary.Add("highres-image", "");
                    dictionary.Add("lowres-video", "");
                    dictionary.Add("highres-video", "");
                    dictionary.Add("description", "");
                    dictionary.Add("description-notags", "");
                    dictionary.Add("tags", "");
                    dictionary.Add("latitude", "");
                    dictionary.Add("longitude", "");
                    dictionary.Add("location", "");

                    if (dict.ContainsKey("id"))
                    {
                        dictionary["id"] = (string)dict["id"];
                    }

                    if (dict.ContainsKey("link"))
                    {
                        dictionary["link"] = (string)dict["link"];
                    }

                    if (dict.ContainsKey("videos"))
                    {
                        dictionary["video"] = "";
                        foreach (KeyValuePair<string, object> kvp in (Dictionary<string, object>)dict["videos"])
                        {
                            if (kvp.Key == "low_bandwidth")
                            {
                                dictionary["lowres-video"] = (string)kvp.Value;
                            }
                            if (kvp.Key == "standard_resolution")
                            {
                                dictionary["highres-video"] = (string)kvp.Value;
                            }
                        }
                    }

                    if (dict.ContainsKey("location"))
                    {
                        foreach (KeyValuePair<string, object> kvp in (Dictionary<string, object>)dict["location"])
                        {
                            if (kvp.Key == "latitude")
                            {
                                dictionary["latitude"] = (string)kvp.Value;
                            }
                            if (kvp.Key == "longitude")
                            {
                                dictionary["longitude"] = (string)kvp.Value;
                            }
                            if (kvp.Key == "name")
                            {
                                dictionary["location"] = (string)kvp.Value;
                            }
                        }
                    }

                    if (dict.ContainsKey("images"))
                    {
                        foreach (KeyValuePair<string, object> kvp in (Dictionary<string, object>)dict["images"])
                        {
                            if (kvp.Key == "thumbnail")
                            {
                                dictionary["thumbnail-image"] = (string)kvp.Value;
                            }
                            if (kvp.Key == "low_resolution")
                            {
                                dictionary["lowres-image"] = (string)kvp.Value;
                            }
                            if (kvp.Key == "standard_resolution")
                            {
                                dictionary["highres-image"] = (string)kvp.Value;
                            }
                        }
                    }

                    if (dict.ContainsKey("caption"))
                    {
                        foreach (KeyValuePair<string, object> kvp in (Dictionary<string, object>)dict["caption"])
                        {
                            if (kvp.Key == "text")
                            {
                                dictionary["description"] = (string)kvp.Value;
                                dictionary["description-notags"] = (string)kvp.Value;
                            }
                        }
                    }

                    if (dict.ContainsKey("tags"))
                    {
                        foreach (KeyValuePair<string, object> kvp in (Dictionary<string, object>)dict["tags"])
                        {

                        }
                    }

                    list.Add(dictionary);
                }

                return list;
            }
        }
    }
}
