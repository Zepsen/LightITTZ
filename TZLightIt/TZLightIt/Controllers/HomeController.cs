using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

using TZLightIt.Domain;
using TZLightIt.Models;
using Cesar = TZLightIt.Models.CesarJSON;

namespace TZLightIt.Controllers
{
    public class HomeController : Controller
    {
        //Entity Framework
        private cesarEntities1 db = new cesarEntities1();

        public ActionResult Index()
        {
            return View();
        }

        public string Crypting(string text, int step)
        {
            var res = "";

            foreach (var letter in text)
            {
                var asciiCode = Convert.ToInt32(letter);
                var temp = 0;
                //only uppercase letters
                if (asciiCode > 64 && asciiCode < 92)
                {
                    temp = ((asciiCode - 65) + step) % 26;
                    asciiCode = (temp < 0) 
                        ? 26 - Math.Abs(temp) + 65
                        : temp + 65;
                }
                //only lowercase letters
                else if (asciiCode > 96 && asciiCode < 123)
                {
                    temp = ((asciiCode - 97) + step) % 26;
                    asciiCode = (temp < 0) 
                        ? 26 - Math.Abs(temp) + 97
                        : temp + 97;
                }
                res += Convert.ToChar(asciiCode);
            }

            return res;
        }

        /*Сount letters in the text,
         * and write the result into an object
         * */
        public List<Letters> CountLetters(string text)
        {
            var res = new Dictionary<char, int>();
            foreach (var letter in text)
            {
                var asciiCode = Convert.ToInt32(letter);
                if (char.IsLetter(letter) && 
                   (asciiCode > 64 && asciiCode < 91 || asciiCode > 96 && asciiCode < 123))
                {
                    if (res.ContainsKey(letter))
                    {
                        res[letter]++;
                    }
                    else
                    {
                        res.Add(letter, 1);
                    }
                }
            }


            var lettersList = res
                      .Select(item => new Letters // LINQ create New Letters
                            {
                                Label = item.Key,   //label,value - for diagram
                                Value = item.Value
                            })
                      .OrderByDescending(i => i.Value).ThenBy(i => i.Label) //Sort by Counts then By Letter
                      .ToList();

            return lettersList;
        }


        private void SaveToDB(CesarJSON obj, string newData)
        {
            //Create anonym entities Cesar and add to db
            db.Cesars.Add(new Domain.Cesar
            {
                Text = obj.Data,
                Results = newData,
                Step = obj.Step,
                Method = obj.Method
            });

            db.SaveChanges();
        }



        /*Ajax method*/
        public JsonResult Main(CesarJSON obj)
        {
            obj.Step = Math.Abs(obj.Step);

            //if data empty
            if (!string.IsNullOrEmpty(obj.Data))
            {
                //Choose method and return decode/encode data
                var newData = (obj.Method == "Decode")
                    ? Crypting(obj.Data, obj.Step)
                    : Crypting(obj.Data, -obj.Step);

                //Save to db
                SaveToDB(obj, newData);

                //System message
                if (obj.Data != newData)
                {
                    obj.Message = (obj.Method == "Decode")
                        ? "Decode complete!"
                        : "Encode complete!";
                }
                else
                {
                    obj.Message = (obj.Method == "Decode")
                        ? "Decode fail! No data to Decoding"
                        : "Encode fail! No data to Encoding";
                }

                //Call method CountLetters that returns List of object Letters  and serialize to JSON; 
                obj.Data = newData;
                obj.Letters = JsonConvert.SerializeObject(CountLetters(newData));
            }
            else
            {
                obj.Message = "Please enter text!";
            }

            //Serialize obj(CesarJSON) to Json
            var jsonResult = JsonConvert.SerializeObject(obj);
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


    }
}