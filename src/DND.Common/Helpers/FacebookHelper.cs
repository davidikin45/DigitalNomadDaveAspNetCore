using Microsoft.VisualBasic.CompilerServices;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace Solution.Base.Helpers
{
    public class Google
    {
        public class Address
        {
            public static System.Collections.Generic.Dictionary<string, string> Search(string address, string latitude, string longitude, string region = "au")
            {
                System.Collections.Generic.Dictionary<string, string> result;
                if (Operators.CompareString(address, "", false) != 0)
                {
                    result = JSONHelper.Google.Address.SearchByAddress(address, region);
                }
                else
                {
                    result = JSONHelper.Google.Address.SearchByLatitudeLongitude(latitude, longitude);
                }
                return result;
            }

            public static System.Collections.Generic.Dictionary<string, string> SearchByAddress(string address, string region = "au")
            {
                System.Collections.ArrayList result = JSONHelper.Google.Address.QueryAPI("address=" + address.Replace(" ", "+") + "&region=" + region);
                return JSONHelper.Google.Address.GetAddressComponentsFromResult(result);
            }

            public static System.Collections.Generic.Dictionary<string, string> SearchByLatitudeLongitude(string latitude, string longitude)
            {
                System.Collections.ArrayList result = JSONHelper.Google.Address.QueryAPI("latlng=" + latitude + "," + longitude);
                return JSONHelper.Google.Address.GetAddressComponentsFromResult(result);
            }

            public static string GetFormattedAddress(string latitude, string longitude)
            {
                System.Collections.ArrayList result = JSONHelper.Google.Address.QueryAPI("latlng=" + latitude + "," + longitude);
                System.Collections.Generic.Dictionary<string, string> addressComponentsFromResult = JSONHelper.Google.Address.GetAddressComponentsFromResult(result);
                return addressComponentsFromResult["formatted"];
            }

            public static void GetLatitudeLongitude(string address, ref string latitude, ref string longitude, string region = "au")
            {
                System.Collections.ArrayList result = JSONHelper.Google.Address.QueryAPI("address=" + address.Replace(" ", "+") + "&region=" + region);
                System.Collections.Generic.Dictionary<string, string> addressComponentsFromResult = JSONHelper.Google.Address.GetAddressComponentsFromResult(result);
                latitude = addressComponentsFromResult["latitude"];
                longitude = addressComponentsFromResult["longitude"];
            }

            public static System.Collections.ArrayList QueryAPI(string parameters)
            {
                string jsonString = JSONHelper.Get(string.Format("https://maps.googleapis.com/maps/api/geocode/json?{0}&sensor=false", parameters));
                System.Collections.Generic.Dictionary<string, object> dictionary = JSONHelper.DeserializeToDictionary(jsonString);
                return (System.Collections.ArrayList)dictionary["results"];
            }

            private static System.Collections.Generic.Dictionary<string, string> GetAddressComponentsFromResult(System.Collections.ArrayList result)
            {
                System.Collections.Generic.Dictionary<string, string> dictionary = new System.Collections.Generic.Dictionary<string, string>();
                dictionary.Add("formatted", "");
                dictionary.Add("formatted-multiline", "");
                dictionary.Add("streetnumber", "");
                dictionary.Add("street", "");
                dictionary.Add("suburb", "");
                dictionary.Add("state", "");
                dictionary.Add("postcode", "");
                dictionary.Add("country", "");
                dictionary.Add("latitude", "");
                dictionary.Add("longitude", "");
                System.Collections.ArrayList arrayList = null;
                if (result.Count > 0)
                {
                    dictionary["formatted"] = Conversions.ToString(NewLateBinding.LateIndexGet(result[0], new object[]
                    {
                            "formatted_address"
                    }, null));
                    dictionary["formatted-multiline"] = dictionary["formatted"].Replace(", ", System.Environment.NewLine);
                    arrayList = (System.Collections.ArrayList)NewLateBinding.LateIndexGet(result[0], new object[]
                    {
                            "address_components"
                    }, null);
                    dictionary["latitude"] = Conversions.ToString(NewLateBinding.LateIndexGet(NewLateBinding.LateIndexGet(NewLateBinding.LateIndexGet(result[0], new object[]
                    {
                            "geometry"
                    }, null), new object[]
                    {
                            "location"
                    }, null), new object[]
                    {
                            "lat"
                    }, null));
                    dictionary["longitude"] = Conversions.ToString(NewLateBinding.LateIndexGet(NewLateBinding.LateIndexGet(NewLateBinding.LateIndexGet(result[0], new object[]
                    {
                            "geometry"
                    }, null), new object[]
                    {
                            "location"
                    }, null), new object[]
                    {
                            "lng"
                    }, null));
                }
                if (arrayList != null)
                {
                    try
                    {
                        System.Collections.IEnumerator enumerator = arrayList.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object objectValue = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(enumerator.Current);
                            System.Collections.Generic.Dictionary<string, object> dictionary2 = (System.Collections.Generic.Dictionary<string, object>)objectValue;
                            string value = Conversions.ToString(dictionary2["long_name"]);
                            System.Collections.ArrayList arrayList2 = (System.Collections.ArrayList)dictionary2["types"];
                            try
                            {
                                System.Collections.IEnumerator enumerator2 = arrayList2.GetEnumerator();
                                while (enumerator2.MoveNext())
                                {
                                    object objectValue2 = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(enumerator2.Current);
                                    object left = objectValue2;
                                    if (Operators.ConditionalCompareObjectEqual(left, "street_number", false))
                                    {
                                        dictionary["streetnumber"] = value;
                                    }
                                    else if (Operators.ConditionalCompareObjectEqual(left, "route", false))
                                    {
                                        dictionary["street"] = value;
                                    }
                                    else if (Operators.ConditionalCompareObjectEqual(left, "locality", false))
                                    {
                                        dictionary["suburb"] = value;
                                    }
                                    else if (Operators.ConditionalCompareObjectEqual(left, "administrative_area_level_1", false))
                                    {
                                        dictionary["state"] = value;
                                    }
                                    else if (Operators.ConditionalCompareObjectEqual(left, "postal_code", false))
                                    {
                                        dictionary["postcode"] = value;
                                    }
                                    else if (Operators.ConditionalCompareObjectEqual(left, "country", false))
                                    {
                                        dictionary["country"] = value;
                                    }
                                }
                            }
                            finally
                            {
                                System.Collections.IEnumerator enumerator2;
                                if (enumerator2 is System.IDisposable)
                                {
                                    (enumerator2 as System.IDisposable).Dispose();
                                }
                            }
                        }
                    }
                    finally
                    {
                        System.Collections.IEnumerator enumerator;
                        if (enumerator is System.IDisposable)
                        {
                            (enumerator as System.IDisposable).Dispose();
                        }
                    }
                }
                return dictionary;
            }
        }

        public class URLShortener
        {
            public static string ShortenURL(string address)
            {
                string parameters = "{\"longUrl\": \"" + address + "\"}";
                System.Collections.Generic.Dictionary<string, object> dictionary = JSONHelper.Google.URLShortener.QueryAPI(parameters);
                string result = "";
                if (dictionary.ContainsKey("id"))
                {
                    result = Conversions.ToString(dictionary["id"]);
                }
                return result;
            }

            public static System.Collections.Generic.Dictionary<string, object> QueryAPI(string parameters)
            {
                string jsonString = JSONHelper.Post("https://www.googleapis.com/urlshortener/v1/url", parameters);
                return JSONHelper.DeserializeToDictionary(jsonString);
            }
        }

        public class MapsEmbed
        {
            public enum MapsEmbedMapType
            {
                Satellite,
                Roadmap
            }

            public static string GetEmbedURL(string APIKey, string location, string latitude, string longitude, JSONHelper.Google.MapsEmbed.MapsEmbedMapType mapType = JSONHelper.Google.MapsEmbed.MapsEmbedMapType.Roadmap, int zoom = 10)
            {
                string text = "";
                if (Operators.CompareString(location, "", false) != 0)
                {
                    text = string.Format("https://www.google.com/maps/embed/v1/place?key={0}", APIKey);
                    text = text + "&q=" + location.Replace(" ", "+");
                    if (Operators.CompareString(latitude, "", false) != 0 && Operators.CompareString(longitude, "", false) != 0)
                    {
                        text = string.Concat(new string[]
                        {
                                text,
                                "&center=",
                                latitude,
                                ",",
                                longitude
                        });
                    }
                    text = text + "&zoom=" + zoom.ToString();
                    if (mapType == JSONHelper.Google.MapsEmbed.MapsEmbedMapType.Roadmap)
                    {
                        text += "&maptype=roadmap";
                    }
                    else if (mapType == JSONHelper.Google.MapsEmbed.MapsEmbedMapType.Satellite)
                    {
                        text += "&maptype=satellite";
                    }
                }
                else if (Operators.CompareString(latitude, "", false) != 0 && Operators.CompareString(longitude, "", false) != 0)
                {
                    text = string.Format("https://www.google.com/maps/embed/v1/view?key={0}", APIKey);
                    text = string.Concat(new string[]
                    {
                            text,
                            "&center=",
                            latitude,
                            ",",
                            longitude
                    });
                    text = text + "&zoom=" + zoom.ToString();
                    if (mapType == JSONHelper.Google.MapsEmbed.MapsEmbedMapType.Roadmap)
                    {
                        text += "&maptype=roadmap";
                    }
                    else if (mapType == JSONHelper.Google.MapsEmbed.MapsEmbedMapType.Satellite)
                    {
                        text += "&maptype=satellite";
                    }
                }
                return text;
            }

            public static System.Collections.ArrayList QueryUser(string clientID, string userID)
            {
                string jsonString = JSONHelper.Get(string.Format("https://api.instagram.com/v1/users/{0}/media/recent/?client_id={1}", userID, clientID));
                System.Collections.Generic.Dictionary<string, object> dictionary = JSONHelper.DeserializeToDictionary(jsonString);
                return (System.Collections.ArrayList)dictionary["data"];
            }
        }
    }

    public class Instagram
    {
        public class User
        {
            public static System.Collections.Generic.Dictionary<string, string> GetUserPostById(string clientID, string userID, string id)
            {
                try
                {
                    System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>.Enumerator enumerator = JSONHelper.Instagram.User.GetUserLatestPosts(clientID, userID).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        System.Collections.Generic.Dictionary<string, string> current = enumerator.Current;
                        if (Operators.CompareString(current["id"], id, false) == 0)
                        {
                            return current;
                        }
                    }
                }
                finally
                {
                    System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>.Enumerator enumerator;
                    ((System.IDisposable)enumerator).Dispose();
                }
                return null;
            }

            public static System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>> GetUserLatestPosts(string clientID, string userID)
            {
                CacheObject<System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>> cacheObject = new CacheObject<System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>>(CacheAction.Add, userID, 10);
                System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>> list = cacheObject.GetObject();
                if (list == null && userID != null)
                {
                    System.Collections.ArrayList result = JSONHelper.Instagram.User.QueryUser(clientID, userID, "");
                    list = JSONHelper.Instagram.User.GetUserLatestPostsFromResult(result);
                    list = cacheObject.SetObject(list);
                }
                return list;
            }

            public static System.Collections.ArrayList QueryUser(string clientID, string userID, string url = "")
            {
                string jsonString;
                if (Operators.CompareString(url, "", false) == 0)
                {
                    jsonString = JSONHelper.Get(string.Format("https://api.instagram.com/v1/users/{0}/media/recent/?client_id={1}", userID, clientID));
                }
                else
                {
                    jsonString = JSONHelper.Get(string.Format(url, new object[0]));
                }
                System.Collections.Generic.Dictionary<string, object> dictionary = JSONHelper.DeserializeToDictionary(jsonString);
                System.Collections.ArrayList arrayList = (System.Collections.ArrayList)dictionary["data"];
                if (dictionary.ContainsKey("pagination"))
                {
                    System.Collections.Generic.Dictionary<string, object> dictionary2 = (System.Collections.Generic.Dictionary<string, object>)dictionary["pagination"];
                    if (dictionary2.ContainsKey("next_url"))
                    {
                        string url2 = Conversions.ToString(dictionary2["next_url"]);
                        arrayList.AddRange(JSONHelper.Instagram.User.QueryUser("", "", url2));
                    }
                }
                return arrayList;
            }

            public static string GetUserId(string clientID, string userName)
            {
                System.Collections.Generic.Dictionary<string, object> user = JSONHelper.Instagram.User.GetUser(clientID, userName);
                if (user != null && user.ContainsKey("id"))
                {
                    return Conversions.ToString(JSONHelper.Instagram.User.GetUser(clientID, userName)["id"]);
                }
                return "";
            }

            public static string GetUserProfilePicture(string clientID, string userName)
            {
                System.Collections.Generic.Dictionary<string, object> user = JSONHelper.Instagram.User.GetUser(clientID, userName);
                if (user != null && user.ContainsKey("profile_picture"))
                {
                    return Conversions.ToString(JSONHelper.Instagram.User.GetUser(clientID, userName)["profile_picture"]);
                }
                return "";
            }

            public static System.Collections.Generic.Dictionary<string, object> GetUser(string clientID, string userName)
            {
                CacheObject<System.Collections.Generic.Dictionary<string, object>> cacheObject = new CacheObject<System.Collections.Generic.Dictionary<string, object>>(CacheAction.Add, userName, 10);
                System.Collections.Generic.Dictionary<string, object> dictionary = cacheObject.GetObject();
                if (dictionary == null && userName != null)
                {
                    dictionary = (System.Collections.Generic.Dictionary<string, object>)JSONHelper.Instagram.User.QueryUserId(clientID, userName)[0];
                    dictionary = cacheObject.SetObject(dictionary);
                }
                return dictionary;
            }

            public static System.Collections.ArrayList QueryUserId(string clientID, string userName)
            {
                string jsonString = JSONHelper.Get(string.Format("https://api.instagram.com/v1/users/search?q={0}&client_id={1}", userName, clientID));
                System.Collections.Generic.Dictionary<string, object> dictionary = JSONHelper.DeserializeToDictionary(jsonString);
                return (System.Collections.ArrayList)dictionary["data"];
            }

            private static System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>> GetUserLatestPostsFromResult(System.Collections.ArrayList result)
            {
                System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>> list = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>();
                try
                {
                    try
                    {
                        System.Collections.IEnumerator enumerator = result.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object objectValue = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(enumerator.Current);
                            System.Collections.Generic.Dictionary<string, string> dictionary = new System.Collections.Generic.Dictionary<string, string>();
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
                            if (Conversions.ToBoolean(NewLateBinding.LateGet(objectValue, null, "containskey", new object[]
                            {
                                    "id"
                            }, null, null, null)))
                            {
                                dictionary["id"] = Conversions.ToString(NewLateBinding.LateIndexGet(objectValue, new object[]
                                {
                                        "id"
                                }, null));
                            }
                            if (Conversions.ToBoolean(NewLateBinding.LateGet(objectValue, null, "containskey", new object[]
                            {
                                    "link"
                            }, null, null, null)))
                            {
                                dictionary["link"] = Conversions.ToString(NewLateBinding.LateIndexGet(objectValue, new object[]
                                {
                                        "link"
                                }, null));
                            }
                            if (Conversions.ToBoolean(NewLateBinding.LateGet(objectValue, null, "containskey", new object[]
                            {
                                    "videos"
                            }, null, null, null)))
                            {
                                dictionary.Add("video", "");
                                System.Collections.Generic.Dictionary<string, object> dictionary2 = (System.Collections.Generic.Dictionary<string, object>)NewLateBinding.LateIndexGet(objectValue, new object[]
                                {
                                        "videos"
                                }, null);
                                try
                                {
                                    System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator2 = dictionary2.GetEnumerator();
                                    while (enumerator2.MoveNext())
                                    {
                                        System.Collections.Generic.KeyValuePair<string, object> current = enumerator2.Current;
                                        string key = current.Key;
                                        if (Operators.CompareString(key, "low_bandwidth", false) == 0)
                                        {
                                            System.Collections.Generic.Dictionary<string, object> dictionary3 = (System.Collections.Generic.Dictionary<string, object>)current.Value;
                                            dictionary["lowres-video"] = Conversions.ToString(dictionary3["url"]);
                                        }
                                        else if (Operators.CompareString(key, "standard_resolution", false) == 0)
                                        {
                                            System.Collections.Generic.Dictionary<string, object> dictionary4 = (System.Collections.Generic.Dictionary<string, object>)current.Value;
                                            dictionary["highres-video"] = Conversions.ToString(dictionary4["url"]);
                                        }
                                    }
                                }
                                finally
                                {
                                    System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator2;
                                    ((System.IDisposable)enumerator2).Dispose();
                                }
                            }
                            if (Conversions.ToBoolean((!Conversions.ToBoolean(NewLateBinding.LateGet(objectValue, null, "containskey", new object[]
                            {
                                    "location"
                            }, null, null, null)) || !Conversions.ToBoolean(NewLateBinding.LateIndexGet(objectValue, new object[]
                            {
                                    "location"
                            }, null) != null)) ? false : true))
                            {
                                System.Collections.Generic.Dictionary<string, object> dictionary5 = (System.Collections.Generic.Dictionary<string, object>)NewLateBinding.LateIndexGet(objectValue, new object[]
                                {
                                        "location"
                                }, null);
                                try
                                {
                                    System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator3 = dictionary5.GetEnumerator();
                                    while (enumerator3.MoveNext())
                                    {
                                        System.Collections.Generic.KeyValuePair<string, object> current2 = enumerator3.Current;
                                        string key2 = current2.Key;
                                        if (Operators.CompareString(key2, "latitude", false) == 0)
                                        {
                                            dictionary["latitude"] = Conversions.ToString(current2.Value);
                                        }
                                        else if (Operators.CompareString(key2, "longitude", false) == 0)
                                        {
                                            dictionary["longitude"] = Conversions.ToString(current2.Value);
                                        }
                                        else if (Operators.CompareString(key2, "name", false) == 0)
                                        {
                                            dictionary["location"] = Conversions.ToString(current2.Value);
                                        }
                                    }
                                }
                                finally
                                {
                                    System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator3;
                                    ((System.IDisposable)enumerator3).Dispose();
                                }
                            }
                            if (NewLateBinding.LateGet(objectValue, null, "containskey", new object[]
                            {
                                    "images"
                            }, null, null, null) != null)
                            {
                                System.Collections.Generic.Dictionary<string, object> dictionary6 = (System.Collections.Generic.Dictionary<string, object>)NewLateBinding.LateIndexGet(objectValue, new object[]
                                {
                                        "images"
                                }, null);
                                try
                                {
                                    System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator4 = dictionary6.GetEnumerator();
                                    while (enumerator4.MoveNext())
                                    {
                                        System.Collections.Generic.KeyValuePair<string, object> current3 = enumerator4.Current;
                                        string key3 = current3.Key;
                                        if (Operators.CompareString(key3, "thumbnail", false) == 0)
                                        {
                                            System.Collections.Generic.Dictionary<string, object> dictionary7 = (System.Collections.Generic.Dictionary<string, object>)current3.Value;
                                            dictionary["thumbnail-image"] = Conversions.ToString(dictionary7["url"]);
                                        }
                                        else if (Operators.CompareString(key3, "low_resolution", false) == 0)
                                        {
                                            System.Collections.Generic.Dictionary<string, object> dictionary8 = (System.Collections.Generic.Dictionary<string, object>)current3.Value;
                                            dictionary["lowres-image"] = Conversions.ToString(dictionary8["url"]);
                                        }
                                        else if (Operators.CompareString(key3, "standard_resolution", false) == 0)
                                        {
                                            System.Collections.Generic.Dictionary<string, object> dictionary9 = (System.Collections.Generic.Dictionary<string, object>)current3.Value;
                                            dictionary["highres-image"] = Conversions.ToString(dictionary9["url"]);
                                        }
                                    }
                                }
                                finally
                                {
                                    System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator4;
                                    ((System.IDisposable)enumerator4).Dispose();
                                }
                            }
                            if (NewLateBinding.LateGet(objectValue, null, "containskey", new object[]
                            {
                                    "caption"
                            }, null, null, null) != null)
                            {
                                System.Collections.Generic.Dictionary<string, object> dictionary10 = (System.Collections.Generic.Dictionary<string, object>)NewLateBinding.LateIndexGet(objectValue, new object[]
                                {
                                        "caption"
                                }, null);
                                try
                                {
                                    System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator5 = dictionary10.GetEnumerator();
                                    while (enumerator5.MoveNext())
                                    {
                                        System.Collections.Generic.KeyValuePair<string, object> current4 = enumerator5.Current;
                                        string key4 = current4.Key;
                                        if (Operators.CompareString(key4, "text", false) == 0)
                                        {
                                            dictionary["description"] = Conversions.ToString(current4.Value);
                                            dictionary["description-notags"] = Conversions.ToString(current4.Value);
                                        }
                                    }
                                }
                                finally
                                {
                                    System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator5;
                                    ((System.IDisposable)enumerator5).Dispose();
                                }
                            }
                            if (NewLateBinding.LateGet(objectValue, null, "containskey", new object[]
                            {
                                    "tags"
                            }, null, null, null) != null)
                            {
                                System.Collections.ArrayList arrayList = (System.Collections.ArrayList)NewLateBinding.LateIndexGet(objectValue, new object[]
                                {
                                        "tags"
                                }, null);
                                bool flag = false;
                                try
                                {
                                    System.Collections.Generic.IEnumerator<object> enumerator6 = (from f in arrayList.ToArray().ToList<object>()
                                                                                                  orderby f.ToString().Length descending
                                                                                                  select f).GetEnumerator();
                                    while (enumerator6.MoveNext())
                                    {
                                        object objectValue2 = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(enumerator6.Current);
                                        dictionary["tags"] = Conversions.ToString(Operators.AddObject(Operators.AddObject(dictionary["tags"] + "#", objectValue2), ","));
                                        dictionary["description-notags"] = dictionary["description-notags"].Replace(Conversions.ToString(Operators.AddObject(" #", objectValue2)), "", System.StringComparison.OrdinalIgnoreCase);
                                        flag = true;
                                    }
                                }
                                finally
                                {
                                    System.Collections.Generic.IEnumerator<object> enumerator6;
                                    if (enumerator6 != null)
                                    {
                                        enumerator6.Dispose();
                                    }
                                }
                                if (flag)
                                {
                                    dictionary["tags"] = dictionary["tags"].Substring(0, checked(dictionary["tags"].Length - 1));
                                }
                            }
                            list.Add(dictionary);
                        }
                    }
                    finally
                    {
                        System.Collections.IEnumerator enumerator;
                        if (enumerator is System.IDisposable)
                        {
                            (enumerator as System.IDisposable).Dispose();
                        }
                    }
                }
                catch (System.Exception expr_743)
                {
                    ProjectData.SetProjectError(expr_743);
                    ProjectData.ClearProjectError();
                }
                return list;
            }
        }
    }

    public class IPLocation
    {
        public static System.Collections.Generic.Dictionary<string, object> GetLocation(string ipAddress)
        {
            string jsonString = JSONHelper.Get(string.Format("http://freegeoip.net/json/{0}", ipAddress));
            return JSONHelper.DeserializeToDictionary(jsonString);
        }
    }


    public static System.Collections.Generic.Dictionary<string, object> DeserializeToDictionary(string jsonString)
    {
        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        System.Collections.Generic.Dictionary<string, object> dictionary = javaScriptSerializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(jsonString);
        System.Collections.Generic.Dictionary<string, object> dictionary2 = new System.Collections.Generic.Dictionary<string, object>();
        try
        {
            System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator = dictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                System.Collections.Generic.KeyValuePair<string, object> current = enumerator.Current;
                if (current.Value.GetType() == dictionary.GetType())
                {
                    dictionary2.Add(current.Key, System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(null, typeof(JSONHelper), "DeserializeToDictionary", new object[]
                    {
                            System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(current.Value)
                    }, null, null, null)));
                }
                else
                {
                    dictionary2.Add(current.Key, System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(current.Value));
                }
            }
        }
        finally
        {
            System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator;
            ((System.IDisposable)enumerator).Dispose();
        }
        return dictionary2;
    }

    public static System.Collections.Generic.Dictionary<string, object> DeserializeToDictionary(System.Collections.Generic.Dictionary<string, object> values)
    {
        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        System.Collections.Generic.Dictionary<string, object> dictionary = new System.Collections.Generic.Dictionary<string, object>();
        try
        {
            System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator = values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                System.Collections.Generic.KeyValuePair<string, object> current = enumerator.Current;
                if (current.Value.GetType() == values.GetType())
                {
                    dictionary.Add(current.Key, System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(null, typeof(JSONHelper), "DeserializeToDictionary", new object[]
                    {
                            System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(current.Value)
                    }, null, null, null)));
                }
                else
                {
                    dictionary.Add(current.Key, System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(current.Value));
                }
            }
        }
        finally
        {
            System.Collections.Generic.Dictionary<string, object>.Enumerator enumerator;
            ((System.IDisposable)enumerator).Dispose();
        }
        return dictionary;
    }
}
}

}
