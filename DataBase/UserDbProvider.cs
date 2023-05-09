using Microsoft.EntityFrameworkCore;
using VK_TEST.VK_T_Objects.Entity;

namespace VK_TEST.DataBase
{
    public class UserDbProvider<TModel> : IDbProvider<TModel> where TModel : User
    {
        private AppContext Context { get; set; }

        public UserDbProvider(AppContext context)
        {
            Context = context;
        }

        public List<TModel> GetAll() =>
        Context.Set<TModel>().AsNoTracking().Include(u => u.UserGroup).Include(u => u.UserState).ToList();

        public TModel? Get(int id) =>
        Context.Set<TModel>().AsNoTracking().Include(u => u.UserGroup).Include(u => u.UserState).FirstOrDefault(u => u.Id == id);

        public TModel Create(TModel model)
        {
            Context.Set<TModel>().Add(model);
            Context.SaveChanges();
            return model;
        }

        public TModel Update(TModel model)
        {
            var toUpdate = Context.Set<TModel>().AsNoTracking().Include(u => u.UserGroup).Include(u => u.UserState).FirstOrDefault(m => m.Id == model.Id);
            if (toUpdate != null)
                toUpdate = model;

            Context.Update(toUpdate);
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
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public TModel? GetFirstFromPredict(Func<TModel, bool> predict)
        {
            try
            {
                return Context.Set<TModel>().AsNoTracking().Include(x => x.UserGroup).Include(x => x.UserState).Select(x => x).AsEnumerable().First(x => predict(x));
            }
            catch
            {
                return null;
            }
        }
           

        public List<TModel>? GetListFromPredict(Func<TModel, bool> predict)
        {
            try
            {
                return Context.Set<TModel>().AsNoTracking().Include(x => x.UserGroup).Include(x => x.UserState).Select(x => x).AsEnumerable().Where(x => predict(x)).ToList();
            }
            catch
            {
                return null;
            }
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

        async public Task<TModel?> GetAsyncFirstFromPredict(Func<TModel, bool> predict) =>
           await Task.Run(() => GetFirstFromPredict(predict));

        async public Task<List<TModel>?> GetAsyncListFromPredict(Func<TModel, bool> predict) =>
           await Task.Run(() => GetListFromPredict(predict));
    }
}
