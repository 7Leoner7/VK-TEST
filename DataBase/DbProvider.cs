using Microsoft.EntityFrameworkCore;
using VK_TEST.VK_T_Objects.Entity;

namespace VK_TEST.DataBase
{
    public interface IDbProvider<TModel> where TModel : BaseEntityModel
    {
        #region [Базовые операции с БД]
        public List<TModel> GetAll();
        public TModel? Get(int id);
        public TModel Create(TModel model);
        public TModel Update(TModel model);
        public bool Delete(int id);

        public Task<TModel> UpdateAsync(TModel model);
        public Task<TModel> CreateAsync(TModel model);
        public Task<TModel?> GetAsync(int id);
        public Task<List<TModel>> GetAllAsync();
        public Task<bool> DeleteAsync(int id);
        #endregion

        #region [Операции посика для поиска подходящих элементов]
        public TModel? GetFirstFromPredict(Func<TModel, bool> predict);
        public List<TModel>? GetListFromPredict(Func<TModel, bool> predict);

        public Task<TModel?> GetAsyncFirstFromPredict(Func<TModel, bool> predict);
        public Task<List<TModel>?> GetAsyncListFromPredict(Func<TModel, bool> predict);
        #endregion
    }

    public class DbProvider<TModel> : IDbProvider<TModel> where TModel : BaseEntityModel
    {
        private AppContext Context { get; set; }

        public DbProvider(AppContext context)
        {
            Context = context;
        }

        public TModel Create(TModel model)
        {
            Context.Set<TModel>().Add(model);
            Context.SaveChanges();
            return model;
        }

        public bool Delete(int id)
        {
            try
            {
                var toDelete = Context.Set<TModel>().FirstOrDefault(room => room.Id == id);
                Context.Set<TModel>().Remove(toDelete);
                Context.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
            
        }

        public TModel? Get(int id) =>
            Context.Set<TModel>().FirstOrDefault(m => m.Id == id);

        public List<TModel> GetAll() =>
            Context.Set<TModel>().ToList();

        public TModel Update(TModel model)
        {
            var toUpdate = Context.Set<TModel>().AsNoTracking().FirstOrDefault(m => m.Id == model.Id);
            if (toUpdate != null)
                toUpdate = model;

            Context.Update(toUpdate);
            Context.SaveChanges();
            return model;
        }

        async public Task<TModel> UpdateAsync(TModel model) =>
            await Task.Run(() => Update(model));

        async public Task<TModel> CreateAsync(TModel model) =>
            await Task.Run(() => Create(model));

        async public Task<TModel?> GetAsync(int id) =>
            await Task.Run(() => Get(id));

        async public Task<List<TModel>> GetAllAsync() =>
           await Task.Run(() => GetAll());

        async public Task<bool> DeleteAsync(int id) =>
            await Task.Run(() => Delete(id));

        public TModel? GetFirstFromPredict(Func<TModel, bool> predict) =>
            Context.Set<TModel>().AsNoTracking().FirstOrDefault(x => predict(x));

        public List<TModel>? GetListFromPredict(Func<TModel, bool> predict) =>
            Context.Set<TModel>().AsNoTracking().Where(x => predict(x)).ToList();

        async public Task<TModel?> GetAsyncFirstFromPredict(Func<TModel, bool> predict) =>
           await Task.Run(() => GetFirstFromPredict(predict));

        async public Task<List<TModel>?> GetAsyncListFromPredict(Func<TModel, bool> predict) =>
           await Task.Run(() => GetListFromPredict(predict));
    }
}
