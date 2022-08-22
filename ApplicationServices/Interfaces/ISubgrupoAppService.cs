using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ISubgrupoAppService : IAppServiceBase<SUBGRUPO_PLANO_CONTA>
    {
        Int32 ValidateCreate(SUBGRUPO_PLANO_CONTA item, USUARIO usuario);
        Int32 ValidateEdit(SUBGRUPO_PLANO_CONTA item, SUBGRUPO_PLANO_CONTA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(SUBGRUPO_PLANO_CONTA item, SUBGRUPO_PLANO_CONTA itemAntes);
        Int32 ValidateDelete(SUBGRUPO_PLANO_CONTA item, USUARIO usuario);
        Int32 ValidateReativar(SUBGRUPO_PLANO_CONTA item, USUARIO usuario);

        SUBGRUPO_PLANO_CONTA CheckExist(SUBGRUPO_PLANO_CONTA obj, Int32 idAss);
        List<SUBGRUPO_PLANO_CONTA> GetAllItens(Int32 idAss);
        List<SUBGRUPO_PLANO_CONTA> GetAllItensAdm(Int32 idAss);
        SUBGRUPO_PLANO_CONTA GetItemById(Int32 id);
        List<GRUPO_PLANO_CONTA> GetAllGrupos(Int32 idAss);
    }
}
