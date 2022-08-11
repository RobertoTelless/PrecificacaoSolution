using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICargoRepository : IRepositoryBase<CARGO_USUARIO>
    {
        CARGO_USUARIO CheckExist(CARGO_USUARIO item, Int32 idAss);
        List<CARGO_USUARIO> GetAllItens(Int32 idAss);
        CARGO_USUARIO GetItemById(Int32 id);
        List<CARGO_USUARIO> GetAllItensAdm(Int32 idAss);
    }
}
