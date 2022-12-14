using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICustoFixoAppService : IAppServiceBase<CUSTO_FIXO>
    {
        Int32 ValidateCreate(CUSTO_FIXO perfil, USUARIO usuario, out Int32 conta);
        Int32 ValidateEdit(CUSTO_FIXO perfil, CUSTO_FIXO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(CUSTO_FIXO item, CUSTO_FIXO itemAntes);
        Int32 ValidateDelete(CUSTO_FIXO perfil, USUARIO usuario, out Int32 conta);
        Int32 ValidateReativar(CUSTO_FIXO perfil, USUARIO usuario, out Int32 conta);

        List<CUSTO_FIXO> GetAllItens(Int32 idAss);
        List<CUSTO_FIXO> GetAllItensAdm(Int32 idAss);
        CUSTO_FIXO GetItemById(Int32 id);
        CUSTO_FIXO CheckExist(CUSTO_FIXO conta, Int32 idAss);

        List<CATEGORIA_CUSTO_FIXO> GetAllTipos(Int32 idAss);
        List<PERIODICIDADE_TAREFA> GetAllPeriodicidades(Int32 idAss);

        Int32 ExecuteFilter(Int32? catId, String nome, DateTime? dataInicio, DateTime? dataFinal, Int32 idAss, out List<CUSTO_FIXO> objeto);
        
    }
}
