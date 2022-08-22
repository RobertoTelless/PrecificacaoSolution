using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICentroCustoService : IServiceBase<PLANO_CONTA>
    {
        Int32 Create(PLANO_CONTA perfil, LOG log);
        Int32 Create(PLANO_CONTA perfil);
        Int32 Edit(PLANO_CONTA perfil, LOG log);
        Int32 Edit(PLANO_CONTA perfil);
        Int32 Delete(PLANO_CONTA perfil, LOG log);

        List<PLANO_CONTA> ExecuteFilter(Int32? grupoId, Int32? subGrupoId, Int32? tipo, Int32? movimento, String numero, String nome, Int32 idAss);
        PLANO_CONTA GetItemById(Int32 id);
        List<PLANO_CONTA> GetAllItens(Int32 idAss);
        List<PLANO_CONTA> GetAllItensAdm(Int32 idAss);
        PLANO_CONTA CheckExist(PLANO_CONTA item, Int32 idAss);
        List<PLANO_CONTA> GetAllDespesas(Int32 idAss);
        List<PLANO_CONTA> GetAllReceitas(Int32 idAss);
    }
}
