using FakeLab;
using System.Linq.Expressions;

namespace AutoLayer.Test
{
    public class MyService(MyContext context)
    {
        private readonly Repository<WeatherForecast> _repo = new(context);
        private readonly Generator _generator = new();

        public async Task<WeatherForecastModel> GetByIdAsync()
        {
            var entity = await _repo.GetByIdAsync(2);

            return AutoMapper.MapToModel<WeatherForecastModel>(entity);
        }

        public async Task<IEnumerable<WeatherForecastModel>> GetWhereAsync()
        {
            var entities = await _repo.GetWhereAsync(w => w.TemperatureC > 7);

            return entities.Select(AutoMapper.MapToModel<WeatherForecastModel>);
        }

        public async Task<IEnumerable<WeatherForecastModel>> GetOrderedAsync()
        {
            var entites = await _repo.GetOrderedAsync(w => w.TemperatureC);

            return entites.Select(AutoMapper.MapToModel<WeatherForecastModel>);
        }

        public async Task<IEnumerable<WeatherForecastModel>> GetPagedAsync()
        {
            var entites = await _repo.GetPagedAsync(1, 10);

            return entites.Select(AutoMapper.MapToModel<WeatherForecastModel>);
        }

        public async Task<bool> ExistsAsync()
        {
            return await _repo.ExistsAsync(x => x.TemperatureC < 0);
        }

        public async Task<int> CountAsync()
        {
            return await _repo.CountAsync();
        }

        public async Task<int> CountWhereAsync()
        {
            return await _repo.CountWhereAsync(x => x.TemperatureC < 0);
        }

        public async Task AddAsync()
        {
            var entity = _generator.GenerateEntity<WeatherForecast>();
            entity.Id = 0;
            entity.TemperatureC = _generator.GenerateNumericValue(-10, 10);
            entity.TemperatureF = 32 + (int)(entity.TemperatureC / 0.5556);

            await _repo.AddAsync(entity);
        }

        public async Task AddRangeAsync()
        {
            var data = _generator.GenerateList<WeatherForecast>(1000);
            data.ForEach((w) =>
            {
                w.Id = 0;
                w.TemperatureC = _generator.GenerateNumericValue(-10, 10);
                w.TemperatureF = 32 + (int)(w.TemperatureC / 0.5556);
            });

            await _repo.AddRangeAsync(data);
        }

        public async Task UpdateAsync()
        {
            var data = await _repo.GetAllAsync();
            var entity = data.First();
            entity.Date = DateOnly.FromDateTime(DateTime.Now);
            entity.TemperatureC = 1;

            await _repo.UpdateAsync(entity);
        }

        public async Task UpdateByIdAsync()
        {
            await _repo.UpdateByIdAsync(4, w =>
            {
                w.Date = DateOnly.FromDateTime(DateTime.Now);
                w.TemperatureC = 10000;
            });
        }

        public async Task UpdateRangeAsync()
        {
            var data = await _repo.GetAllAsync();
            var entities = data.Take(10).ToList();
            entities.ForEach((w) =>
            {
                w.TemperatureC = _generator.GenerateNumericValue(-10, 10);
                w.TemperatureF = 32 + (int)(w.TemperatureC / 0.5556);
                w.Date = DateOnly.FromDateTime(DateTime.Now);
            });

            await _repo.UpdateRangeAsync(entities);
        }

        public async Task UpdateWhereAsync()
        {
            await _repo.UpdateWhereAsync(x => x.TemperatureC < 0, w =>
            {
                w.TemperatureC = 123;
                w.TemperatureF = 32 + (int)(w.TemperatureC / 0.5556);
                w.Date = DateOnly.FromDateTime(DateTime.Now);
            });
        }

        public async Task RemoveAsync()
        {
            var data = await _repo.GetAllAsync();
            var entity = data.First();

            await _repo.RemoveAsync(entity);
        }

        public async Task RemoveByIdAsync()
        {
            await _repo.RemoveByIdAsync(5);
        }

        public async Task RemoveRangeAsync()
        {
            var data = await _repo.GetAllAsync();
            var entities = data.Take(10).ToList();

            await _repo.RemoveRangeAsync(entities);
        }

        public async Task RemoveWhereAsync()
        {
            await _repo.RemoveWhereAsync(x => x.TemperatureC == 123);
        }

        public async Task ExecuteTransactionAsync()
        {
            await _repo.ExecuteTransactionAsync(async () =>
            {
                await AddRangeAsync();
                await UpdateRangeAsync();
            });
        }

        public async Task<IEnumerable<WeatherForecastModel>> ExecuteSqlRawAsync()
        {
            var response = await _repo.ExecuteSqlRawAsync("SELECT * FROM weather");
            var result = response.Select(AutoMapper.MapToModel<WeatherForecastModel>);

            return result;
        }

        public async Task<int> ExecuteSqlRawCommandAsync()
        {
            return await _repo.ExecuteSqlRawCommandAsync("DELETE FROM weather where tc < 0");
        }
    }
}
