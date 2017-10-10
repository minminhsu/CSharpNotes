﻿using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LongTimeAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"使用 Get 方法呼叫 Web API ，並且會逾期的結果");
            var foo = JsonPutAsync(4).Result;
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            if (foo.Success == true)
            {
                var item = JsonConvert.DeserializeObject<APIData>(foo.Payload.ToString());
                Console.WriteLine($"Id : {item.Id}");
                Console.WriteLine($"Name : {item.Name}");
                Console.WriteLine($"Filename : {item.Filename}");
            }
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Continue...{Environment.NewLine}");
            Console.ReadKey();

            Console.WriteLine($"使用 Get 方法呼叫 Web API ，並且不會逾期的結果");
            foo = JsonPutAsync(6).Result;
            Console.WriteLine($"結果狀態 : {foo.Success}");
            Console.WriteLine($"結果訊息 : {foo.Message}");
            if (foo.Success == true)
            {
                var item = JsonConvert.DeserializeObject<APIData>(foo.Payload.ToString());
                Console.WriteLine($"Id : {item.Id}");
                Console.WriteLine($"Name : {item.Name}");
                Console.WriteLine($"Filename : {item.Filename}");
            }
            Console.WriteLine($"");

            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();

        }

        private static async Task<APIResult> JsonPutAsync(int sec = 4)
        {
            APIResult fooAPIResult;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        client.Timeout = TimeSpan.FromSeconds(sec);
                        #region 呼叫遠端 Web API
                        //string FooUrl = $"http://localhost:53494/api/Upload";
                        string FooUrl = $"http://vulcanwebapi.azurewebsites.net/api/values";
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{FooUrl}/LongTimeGet";
                        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        response = await client.GetAsync(fooFullUrl);
                        #endregion
                        #endregion

                        #region 處理呼叫完成 Web API 之後的回報結果
                        if (response != null)
                        {
                            // 取得呼叫完成 API 後的回報內容
                            String strResult = await response.Content.ReadAsStringAsync();

                            switch (response.StatusCode)
                            {
                                case HttpStatusCode.OK:
                                    #region 狀態碼為 OK
                                    fooAPIResult = JsonConvert.DeserializeObject<APIResult>(strResult, new JsonSerializerSettings { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                                    #endregion
                                    break;

                                default:
                                    fooAPIResult = new APIResult
                                    {
                                        Success = false,
                                        Message = string.Format("Error Code:{0}, Error Message:{1}", response.StatusCode, response.Content),
                                        Payload = null,
                                    };
                                    break;
                            }
                        }
                        else
                        {
                            fooAPIResult = new APIResult
                            {
                                Success = false,
                                Message = "應用程式呼叫 API 發生異常",
                                Payload = null,
                            };
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        fooAPIResult = new APIResult
                        {
                            Success = false,
                            Message = ex.Message,
                            Payload = ex,
                        };
                    }
                }
            }

            return fooAPIResult;
        }
    }
}
