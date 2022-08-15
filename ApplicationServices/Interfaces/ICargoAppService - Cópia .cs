using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICargoAppService : IAppServiceBase<CARGO_USUARIO>
    {
        Int32 ValidateCreate(CARGO_USUARIO item, USUARIO usuario);
        Int32 ValidateEdit(CARGO_USUARIO item, CARGO_USUARIO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CARGO_USUARIO item, CARGO_USUARIO itemAntes);
        Int32 ValidateDelete(CARGO_USUARIO item, USUARIO usuario);
        Int32 ValidateReativar(CARGO_USUARIO item, USUARIO usuario);

        CARGO_USUARIO CheckExist(CARGO_USUARIO item, Int32 idAss);
        List<CARGO_USUARIO> GetAllItens(Int32 idAss);
        CARGO_USUARIO GetItemById(Int32 id);
        List<CARGO_USUARIO> GetAllItensAdm(Int32 idAss);
    }
}
