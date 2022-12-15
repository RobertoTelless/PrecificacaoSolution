using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICategoriaCustoFixoService : IServiceBase<CATEGORIA_CUSTO_FIXO>
    {
        Int32 Create(CATEGORIA_CUSTO_FIXO perfil, LOG log);
        Int32 Create(CATEGORIA_CUSTO_FIXO perfil);
        Int32 Edit(CATEGORIA_CUSTO_FIXO perfil, LOG log);
        Int32 Edit(CATEGORIA_CUSTO_FIXO perfil);
        Int32 Delete(CATEGORIA_CUSTO_FIXO perfil, LOG log);

        CATEGORIA_CUSTO_FIXO CheckExist(CATEGORIA_CUSTO_FIXO item, Int32 idAss);
        CATEGORIA_CUSTO_FIXO GetItemById(Int32 id);
        List<CATEGORIA_CUSTO_FIXO> GetAllItens(Int32 idAss);
        List<CATEGORIA_CUSTO_FIXO> GetAllItensAdm(Int32 idAss);

    }
}
