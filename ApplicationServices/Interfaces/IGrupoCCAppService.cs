using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IGrupoCCAppService : IAppServiceBase<GRUPO_PLANO_CONTA>
    {
        Int32 ValidateCreate(GRUPO_PLANO_CONTA item, USUARIO usuario);
        Int32 ValidateEdit(GRUPO_PLANO_CONTA item, GRUPO_PLANO_CONTA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(GRUPO_PLANO_CONTA item, GRUPO_PLANO_CONTA itemAntes);
        Int32 ValidateDelete(GRUPO_PLANO_CONTA item, USUARIO usuario);
        Int32 ValidateReativar(GRUPO_PLANO_CONTA item, USUARIO usuario);

        GRUPO_PLANO_CONTA CheckExist(GRUPO_PLANO_CONTA obj, Int32 idAss);
        List<GRUPO_PLANO_CONTA> GetAllItens(Int32 idAss);
        List<GRUPO_PLANO_CONTA> GetAllItensAdm(Int32 idAss);
        GRUPO_PLANO_CONTA GetItemById(Int32 id);
    }
}
