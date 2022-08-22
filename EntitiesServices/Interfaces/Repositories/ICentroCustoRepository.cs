using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICentroCustoRepository : IRepositoryBase<PLANO_CONTA>
    {
        PLANO_CONTA CheckExist(PLANO_CONTA item, Int32 idAss);
        PLANO_CONTA GetItemById(Int32 id);
        List<PLANO_CONTA> GetAllItens(Int32 idAss);
        List<PLANO_CONTA> GetAllItensAdm(Int32 idAss);
        List<PLANO_CONTA> GetAllDespesas(Int32 idAss);
        List<PLANO_CONTA> GetAllReceitas(Int32 idAss);
        List<PLANO_CONTA> ExecuteFilter(Int32? grupoId, Int32? subGrupoId, Int32? tipo, Int32? movimento, String numero, String nome, Int32 idAss);
    }
}
