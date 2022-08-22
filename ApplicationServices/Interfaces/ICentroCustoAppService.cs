using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICentroCustoAppService : IAppServiceBase<PLANO_CONTA>
    {
        Int32 ValidateCreate(PLANO_CONTA item, USUARIO usuario);
        Int32 ValidateEdit(PLANO_CONTA item, PLANO_CONTA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(PLANO_CONTA item, USUARIO usuario);
        Int32 ValidateReativar(PLANO_CONTA item, USUARIO usuario);
        
        Int32 ExecuteFilter(Int32? grupoId, Int32? subGrupoId, Int32? tipo, Int32? movimento, String numero, String nome, Int32 idAss, out List<PLANO_CONTA> objeto);
        List<PLANO_CONTA> GetAllItens(Int32 idAss);
        PLANO_CONTA GetItemById(Int32 id);
        List<PLANO_CONTA> GetAllItensAdm(Int32 idAss);
        PLANO_CONTA CheckExist(PLANO_CONTA obj, Int32 idAss);
        List<PLANO_CONTA> GetAllDespesas(Int32 idAss);
        List<PLANO_CONTA> GetAllReceitas(Int32 idAss);
    }
}
