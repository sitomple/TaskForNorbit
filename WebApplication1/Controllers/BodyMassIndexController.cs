using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlTypes;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodyMassIndexController : ControllerBase
    {
        private readonly double minWeight = 2;
        private readonly double maxWeight = 727;
        private readonly double minHeight = 0.67;
        private readonly double maxHeight = 2.72;
        private readonly int minAge = 0;
        private readonly int maxAge = 123;



        /// <summary>
        /// Проверяет коректность данных ввода веса
        /// </summary>
        /// <param name="weight">Вес человека в килограммах</param>
        /// <returns>Резльутат проверки</returns>
        private bool CheckWeightDouble(string weight)
        {
            try
            {
                double weightToInt = double.Parse(weight);
                return true;
            }
            catch
            {
                return false;
            }       
        }


        /// <summary>
        /// Проверяет коректность данных ввода количества прожитых лет
        /// </summary>
        /// <param name="age">Количество прожитого времени в годах</param>
        /// <returns>Резльутат проверки</returns>
        private bool CheckAgeInt(string age)
        {
            try
            {
                int weightToInt = int.Parse(age);
                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// Проверяет коректность данных ввода роста
        /// </summary>
        /// <param name="height">Рост человека в метрах</param>
        /// <returns>Резльутат проверки</returns>
        private bool CheckHeightDouble(string height)
        {
            try
            {
                double heightToInt = double.Parse(height);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Хранит интерпретацию показателей ИМТ
        /// </summary>
        private Dictionary<double, string> WorldStandrtBMI = new Dictionary<double, string>() 
        {
            { 16 , "Pronounced body weight deficiency"},
            { 18.5 , "Insufficient (deficiency) body weight"},
            { 25 , "Standard"},
            { 30 , "Overweight (pre-obesity)"},
            { 35 , "Obesity of the 1st degree"},
            { 40 , "Obesity of the 2nd degree"},
            { 300 , "Obesity of the 3rd degree"},
        };

       
        
        /// <summary>
        /// Проверяет переданные параметры на их валидность
        /// </summary>
        /// <param name="weight">Вес человека в килограммах</param>
        /// <param name="height">Рост человека в метрах</param>
        /// <returns>Возвращает строку с ошибками</returns>
        private string CheckMessage(string weight, string height)
        {
            string message = "";
            if (!CheckWeightDouble(weight))
                message += "Тип параметра введённый в вессе был отличен от принимаемого типа (Double)\n";
            if (!CheckHeightDouble(height))
                message += "Тип параметра введённый в росте был отличен от принимаемого типа (Double)\n";
            
            if (message == "")
            {
                if (double.Parse(weight) > maxWeight || double.Parse(weight) < minWeight)
                    message += $"Проверьте ведённый вами вес он не соответсвует человеческому! \nДопустимые значения: минимальный - {minWeight} ,а максимальный - {maxWeight}\n";
                if (double.Parse(height) > maxHeight || double.Parse(height) < minHeight)
                    message += $"Проверьте ведённый вами рост он не соответсвует человеческому! \nДопустимые значения: минимальный - {minHeight} ,а максимальный - {maxHeight}\n";
            }
            return message;
        }

        /// <summary>
        /// Проверяет переданные параметры на их валидность
        /// </summary>
        /// <param name="weight">Вес человека в килограммах</param>
        /// <param name="height">Рост человека в метрах</param>
        /// <param name="age">Количество прожитого времени человеком в годах</param>
        /// <returns>Возвращает строку с ошибками</returns>
        private string CheckMessage(string weight, string height, string age)
        {
            string message = "";
            if (!CheckWeightDouble(weight))
                message += "Тип параметра введённый в вессе был отличен от принимаемого типа (Double)\n";
            if (!CheckHeightDouble(height))
                message += "Тип параметра введённый в росте был отличен от принимаемого типа (Double)\n";
            if (!CheckAgeInt(age))
                message += "Тип параметра введённый в возрасте был отличен от принимаемого типа (integer)\n";

            if (message == "")
            {
                if (double.Parse(weight) > maxWeight || double.Parse(weight) < minWeight)
                    message += $"Проверьте ведённый вами вес он не соответсвует человеческому! \nДопустимые значения: минимальный - {minWeight} ,а максимальный - {maxWeight}\n";
                if (double.Parse(height) > maxHeight || double.Parse(height) < minHeight)
                    message += $"Проверьте ведённый вами рост он не соответсвует человеческому! \nДопустимые значения: минимальный - {minHeight} ,а максимальный - {maxHeight}\n";
                if (int.Parse(age) > maxAge || int.Parse(age) < minAge)
                    message += $"Проверьте ведённый вами рост он не соответсвует человеческому! \nДопустимые значения: минимальный - {minAge} ,а максимальный - {maxAge}\n";
            }
            return message;
        }



        /// <summary>
        /// Вычисляет ИМТ и формирует json ответ
        /// </summary>
        /// <param name="weight">Вес человека в килограммах</param>
        /// <param name="height">Рост человека в метрах</param>
        /// <returns>Вычесленную информацию</returns>
        ContentBeforeCalculate CalculateBMI(string weight, string height)
        {
            ContentBeforeCalculate resultCheck = new ContentBeforeCalculate();

            string messageProblem = CheckMessage(weight, height);
            if (messageProblem != "")
            {
                resultCheck.ValueCheck = 0;
                resultCheck.Info = messageProblem;
                return resultCheck;
            }

            double intWeight = double.Parse(weight);
            double intHeight = double.Parse(height);
            double answer = intWeight / (intHeight * 2);
            string coment = "";
            foreach (var e in WorldStandrtBMI)
                if (answer < e.Key)
                {
                    coment = e.Value;
                    break;
                }

            BodyMassIndexModel bmi = new BodyMassIndexModel(coment, answer);
            string json = JsonSerializer.Serialize(bmi);
            resultCheck.ValueCheck = 1;
            resultCheck.Info = json;
            resultCheck.BMI = answer;
            return resultCheck; 
        }

        /// <summary>
        /// Вычисляет ИМТ и формирует json ответ
        /// </summary>
        /// <param name="weight">Вес человека в килограммах</param>
        /// <param name="height">Рост человека в метрах</param>
        /// /// <param name="age">Количество прожитого времени человеком в годах</param>
        /// <returns>Вычесленную информацию</returns>
        ContentBeforeCalculate CalculateBMI(string weight, string height, string age)
        {
            ContentBeforeCalculate resultCheck = new ContentBeforeCalculate();

            string messageProblem = CheckMessage(weight, height, age);
            if (messageProblem != "")
            {
                resultCheck.ValueCheck = 0;
                resultCheck.Info = messageProblem;
                return resultCheck;
            }

            double intWeight = double.Parse(weight);
            double intHeight = double.Parse(height);
            double answer = intWeight / (intHeight * 2);
            string coment = "";
            foreach (var e in WorldStandrtBMI)
                if (answer < e.Key)
                {
                    coment = e.Value;
                    break;
                }

            BodyMassIndexModel bmi = new BodyMassIndexModel(coment, answer);
            string json = JsonSerializer.Serialize(bmi);
            resultCheck.ValueCheck = 1;
            resultCheck.Info = json;
            resultCheck.BMI = answer;
            return resultCheck;
        }


        /// <summary>
        /// Передаёт полученную информацию в метод CalculateBMI откуда получает значение
        /// </summary>
        /// <param name="weight">Вес человка в килограммах</param>
        /// <param name="height">Рост человека в метрах</param>
        /// <returns>Возвращает пользователю ИМТ</returns>
        [HttpGet(Name = "Get")]
        public string GetBodyMassIndex(string weight, string height)
        {
            var resultCheck = CalculateBMI(weight, height);
            return resultCheck.Info;
        }



        /// <summary>
        /// Передаёт полученную информацию в метод CalculateBMI и заносит информацию в базу данных
        /// </summary>
        /// <param name="fio">Фамилия имя и отчество человка</param>
        /// <param name="height">Рост человека в метрах</param>
        /// <param name="weight">Вес человка в килограммах</param>
        /// <param name="age">Количество прожитого времени человеком в годах</param>
        /// <returns>Ответ на завпрос</returns>
        [HttpPost(Name = "Post")]
        public string GetBodyMassIndex(string fio, string height, string weight, string age)
        {
            var resultCheck = CalculateBMI(weight, height, age);
            if (resultCheck.ValueCheck == 0)
                return resultCheck.Info;

            List<UserModel> users = new List<UserModel>();
            using (DataBase db = new DataBase())
            {
                users = db.User.ToList();
            }

            bool check = true;
            foreach (var user in users)
                if (user.FIO == fio)
                {
                    check = false;
                    break;
                }
            if (check)
            {
                using (DataBase db = new DataBase())
                {
                    UserModel user1 = new UserModel { id = users.Count+1, FIO = fio, height = double.Parse(height), weight = double.Parse(weight), age = int.Parse(age) };
                    db.User.Add(user1);
                    db.SaveChanges();
                }
            }

            foreach (var user in users)
            {
                if (user.FIO == fio)
                {
                    using (DataBase db = new DataBase())
                    {
                        MBI_userModel bmiUser = new MBI_userModel { id = user.id, bmi = resultCheck.BMI, date = DateTime.UtcNow };
                        db.MBI_user.Add(bmiUser);
                        db.SaveChanges();
                    }
                    return $"{resultCheck.Info}";
                }
            }

            using (DataBase db = new DataBase())
            {
                MBI_userModel imtUser = new MBI_userModel { id = users.Count, bmi = resultCheck.BMI, date = DateTime.UtcNow };
                db.MBI_user.Add(imtUser);
                db.SaveChanges();
            }
            return $"{resultCheck.Info}";

        }

        /// <summary>
        /// Полная информация о рузльтатах вычисления ИМТ 
        /// </summary>
        struct ContentBeforeCalculate
        {
            public int ValueCheck;
            public string Info;
            public double BMI;
        }
    }
}
