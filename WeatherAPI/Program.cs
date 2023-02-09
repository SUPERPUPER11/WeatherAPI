using Newtonsoft.Json;
using System.Text;
using System.Web;
using WeatherAPI;

Console.OutputEncoding = Encoding.UTF8;
Console.Write("Введите название города: ");
var apiKey = "de652bea5fc39e262e384811c44b47a7";
var city = Console.ReadLine();
var client = new HttpClient();

var response = await client.GetAsync(
    $"https://api.openweathermap.org/data/2.5/weather?q={HttpUtility.UrlEncode(city)}&appid={apiKey}&units=metric&lang=ru");

var responseDays = await client.GetAsync(
    $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric&lang=ru");

if (response.IsSuccessStatusCode)
{
    var result = await response.Content.ReadAsStringAsync();
    var model = JsonConvert.DeserializeObject<WeatherModel>(result);
    Console.WriteLine($"Погода на сегодня для города {model.Name}:");
    Console.WriteLine($"Текущая температура: {model.Main.Temp}°, {model.Weather[0].Description}");
    Console.WriteLine($"Ощущается как: {model.Main.FeelsLike}°");
    Console.WriteLine($"Скорость ветра: {model.Wind.Speed} м/с, {Cardinaldirections(model.Wind.Deg)}.");
    Console.WriteLine($"Влажность: {model.Main.Humidity} %");
    Console.WriteLine($"Давление: {model.Main.Pressure} мм рт. ст.");
}
else
{
    Console.WriteLine("Ошибка! Город не найден.");
}

if (responseDays.IsSuccessStatusCode)
{
    var resultDays = await responseDays.Content.ReadAsStringAsync();
    var modelDays = JsonConvert.DeserializeObject<WeatherDays>(resultDays);
    Console.WriteLine("---------------------------------------------------------------------------------------");
    Console.WriteLine($"Прогноз погоды с {DateTime.Now.ToString("d")} до {DateTime.Now.AddDays(3).ToString("d")} для города {modelDays.City.Name}:");
    Console.WriteLine("------------------------------------------------------------------------------------------");
    Console.WriteLine($"{"Дата и день недели",22} | {"Мин. и макс. температура",35} | {"Описание",25} |");
    Console.WriteLine($"{WetherStrToDateToStr(modelDays.List[0].DtTxt),22} | мин.тем. {modelDays.List[0].Main.TempMin,5}°  макс. темп. {modelDays.List[0].Main.TempMax,5}° | {modelDays.List[0].Weather[0].Description,25} |");
    Console.WriteLine($"{WetherStrToDateToStr(modelDays.List[8].DtTxt),22} | мин.тем. {modelDays.List[8].Main.TempMin,5}°  макс. темп. {modelDays.List[8].Main.TempMax,5}° | {modelDays.List[8].Weather[0].Description,25} |");
    Console.WriteLine($"{WetherStrToDateToStr(modelDays.List[16].DtTxt),22} | мин.тем. {modelDays.List[16].Main.TempMin,5}°  макс. темп. {modelDays.List[16].Main.TempMax,5}° | {modelDays.List[16].Weather[0].Description,25} |");
    Console.WriteLine($"{WetherStrToDateToStr(modelDays.List[24].DtTxt),22} | мин.тем. {modelDays.List[18].Main.TempMin,5}°  макс. темп. {modelDays.List[24].Main.TempMax,5}° | {modelDays.List[24].Weather[0].Description,25} |");
    Console.WriteLine("------------------------------------------------------------------------------------------");
    Console.ReadKey();
}

string Cardinaldirections(int wet) =>
    wet switch
    {
        >= 0 and < 23 or >= 338 and <= 360 => "C",
        >= 23 and < 68 => "СВ",
        >= 68 and < 113 => "В",
        >= 113 and < 158 => "ЮВ",
        >= 158 and < 203 => "Ю",
        >= 203 and < 248 => "ЮЗ",
        >= 248 and < 292 => "З",
        >= 292 and < 338 => "CЗ",
        _ => "",
    };
string WetherStrToDateToStr(string str1)
{
    DateTime Date1 = DateTime.Parse(str1);
    string strDay = Date1.ToString("dddd");
    string strDay1 = Date1.ToString("d");
    return strDay1 + " " + strDay;
}