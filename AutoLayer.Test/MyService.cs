using FakeLab;

namespace AutoLayer.Test
{
    public class MyService(MyContext context)
    {
        private readonly Repository<WeatherForecast> _repo = new(context);
        private readonly Generator _generator = new();

        public void Add()
        {
            var data = _generator.GenerateList<WeatherForecast>(1000);
            data.ForEach((w) =>
            {
                w.Id = 0;
                w.TemperatureC = _generator.GenerateNumericValue(-10, 10);
                w.TemperatureF = 32 + (int)(w.TemperatureC / 0.5556);
            });

            _repo.AddRange(data);
        }

        public void Update()
        {
            var data = _repo.GetAll();
            var entity = data.First();
            entity.Date = DateOnly.FromDateTime(DateTime.Now);
            entity.TemperatureC = 1;

            _repo.Update(entity);
        }

        public void Remove()
        {
            var data = _repo.GetAll();
            var entity = data.First();

            _repo.Remove(entity);
        }
    }
}
