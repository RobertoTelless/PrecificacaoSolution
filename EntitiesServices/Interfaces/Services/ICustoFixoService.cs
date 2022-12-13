using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICustoFixoService : IServiceBase<CUSTO_FIXO>
    {
        Int32 Create(CUSTO_FIXO perfil, LOG log);
        Int32 Create(CUSTO_FIXO perfil);
        Int32 Edit(CUSTO_FIXO perfil, LOG log);
        Int32 Edit(CUSTO_FIXO perfil);
        Int32 Delete(CUSTO_FIXO perfil, LOG log);

        CUSTO_FIXO CheckExist(CUSTO_FIXO conta, Int32 idAss);
        CUSTO_FIXO GetItemById(Int32 id);
        List<CUSTO_FIXO> GetAllItens(Int32 idAss);
        List<CUSTO_FIXO> GetAllItensAdm(Int32 idAss);
        List<CATEGORIA_CUSTO_FIXO> GetAllTipos(Int32 idAss);
        List<PERIODICIDADE_TAREFA> GetAllPeriodicidades(Int32 idAss);

        List<CUSTO_FIXO> ExecuteFilter(Int32? catId, String nome, DateTime? dataInicio, DateTime? dataFinal, Int32 idAss);
    }
}
