using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICargoService : IServiceBase<CARGO_USUARIO>
    {
        Int32 Create(CARGO_USUARIO item, LOG log);
        Int32 Create(CARGO_USUARIO item);
        Int32 Edit(CARGO_USUARIO item, LOG log);
        Int32 Edit(CARGO_USUARIO item);
        Int32 Delete(CARGO_USUARIO item, LOG log);

        CARGO_USUARIO CheckExist(CARGO_USUARIO item, Int32 idAss);
        List<CARGO_USUARIO> GetAllItens(Int32 idAss);
        CARGO_USUARIO GetItemById(Int32 id);
        List<CARGO_USUARIO> GetAllItensAdm(Int32 idAss);
    }
}
