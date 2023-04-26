using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Xml.Linq;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticBMIController : ControllerBase
    {
        /// <summary>
        /// Получение данных из табилц User и MBI_user
        /// </summary>
        /// <returns>Подробный список людей и информацию о них</returns>
        private List<Info> ReadTableInfo()
        {
            List<Info> list = new List<Info>();
            using (DataBase db = new DataBase())
            {
                var users = db.User.Join(db.MBI_user, // второй набор
                    u => u.id, // свойство-селектор объекта из первого набора
                    c => c.id, // свойство-селектор объекта из второго набора
                    (u, c) => new // результат
                    {
                        FIO = u.FIO,
                        BMI = c.bmi,
                        Age = u.age
                    });
                foreach (var e in users)
                {
                    Info infos = new Info();
                    infos.FIO = e.FIO;
                    infos.BMI = e.BMI;
                    infos.age = e.Age;
                    list.Add(infos);
                }
            }
            return list;
        }

        /// <summary>
        /// Выдаёт список характеристик ИМТ и процентное отношение
        /// </summary>
        /// <param name="countMinimal">Количество людей с недобором веса</param>
        /// <param name="countNormal">Количество людей у которых в порядке с весом</param>
        /// <param name="countMax">Количество людей с перебором веса</param>
        /// <returns>Список рейтинга</returns>
        private List<RatingModel> GetRatingList(int countMinimal, int countNormal, int countMax)
        {
            List<RatingModel> ratingModels = new List<RatingModel>();
            int min = countMinimal * 100 / (countNormal + countMinimal + countMax);
            int max = countMax * 100 / (countNormal + countMinimal + countMax);
            int norm = countNormal * 100 / (countNormal + countMinimal + countMax);

            RatingModel rmmax = new RatingModel("above the norm", max);
            RatingModel rmmin = new RatingModel("below the norm", min);
            RatingModel rmnorm = new RatingModel("norm", norm);
            ratingModels.Add(rmmax);
            ratingModels.Add(rmmin);
            ratingModels.Add(rmnorm);
            return ratingModels;
        }

        /// <summary>
        /// Выолняет сортировку от большего к меньшему для формирования требуемого ответа
        /// </summary>
        /// <param name="ratingModels"></param>
        /// <returns>Сортируемый список</returns>
        private List<RatingModel> GetSortRating(ref List<RatingModel> ratingModels)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 1; j < 3; j++)
                {
                    if (ratingModels[i].percent < ratingModels[j].percent)
                    {
                        var swap = ratingModels[i];
                        ratingModels[i] = ratingModels[j];
                        ratingModels[j] = swap;
                    }
                }
            }
            return ratingModels;
        }

        /// <summary>
        /// Получает информацию в процентах о людях в разных категориях ИМТ
        /// </summary>
        /// <returns>возвращает список характеристик ИМТ и процентное отношение клиентов в этой категории</returns>
        [HttpGet(Name = "Get2")]
        public string GetBodyMassIndex()
        {
            List<Info> infos = ReadTableInfo();
            int countNormal = 0;
            int countMinimal = 0;
            int countMax = 0;
            foreach (var e in infos)
            {
                if (e.BMI > 29)
                    countMax++;
                else if (e.BMI < 19)
                    countMinimal++;
                else
                    countNormal++;
            }

            List<RatingModel> ratingModels = GetRatingList(countMinimal, countNormal, countMax);

            GetSortRating(ref ratingModels);

            string json = "";

            foreach (var e in ratingModels)
                json += JsonSerializer.Serialize(e)+"\n";
            return json; 
        }

        /// <summary>
        /// Служит для заполнения информации от объедения таблиц User и BMI_user
        /// </summary>
        struct Info
        {
            public string FIO;
            public double BMI;
            public int age;
        }
    }
}
