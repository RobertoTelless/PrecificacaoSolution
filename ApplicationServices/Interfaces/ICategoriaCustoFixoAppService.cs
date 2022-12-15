using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaCustoFixoAppService : IAppServiceBase<CATEGORIA_CUSTO_FIXO>
    {
        Int32 ValidateCreate(CATEGORIA_CUSTO_FIXO item, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_CUSTO_FIXO item, CATEGORIA_CUSTO_FIXO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CATEGORIA_CUSTO_FIXO item, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_CUSTO_FIXO item, USUARIO usuario);

        CATEGORIA_CUSTO_FIXO CheckExist(CATEGORIA_CUSTO_FIXO item, Int32 idAss);
        List<CATEGORIA_CUSTO_FIXO> GetAllItens(Int32 idAss);
        CATEGORIA_CUSTO_FIXO GetItemById(Int32 id);
        List<CATEGORIA_CUSTO_FIXO> GetAllItensAdm(Int32 idAss);
    }
}
