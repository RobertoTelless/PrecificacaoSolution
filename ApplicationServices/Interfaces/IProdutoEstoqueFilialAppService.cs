using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface  IProdutoEstoqueFilialAppService : IAppServiceBase<PRODUTO_ESTOQUE_EMPRESA>
    {
        Int32 ValidateCreate(PRODUTO_ESTOQUE_EMPRESA item, USUARIO usuario);
        Int32 ValidateEdit(PRODUTO_ESTOQUE_EMPRESA item, PRODUTO_ESTOQUE_EMPRESA itemAntes, USUARIO usuario);
        Int32 ValidateEditEstoque(PRODUTO_ESTOQUE_EMPRESA item, PRODUTO_ESTOQUE_EMPRESA itemAntes, USUARIO usuario);

        List<PRODUTO_ESTOQUE_EMPRESA> GetAllItens(Int32 idAss);
        PRODUTO_ESTOQUE_EMPRESA GetByProdFilial(Int32 prod, Int32 fili);
        List<PRODUTO_ESTOQUE_EMPRESA> GetByProd(Int32 id);
        PRODUTO_ESTOQUE_EMPRESA CheckExist(PRODUTO_ESTOQUE_EMPRESA item, Int32 idAss);
        PRODUTO_ESTOQUE_EMPRESA GetItemById(Int32 id);
        PRODUTO_ESTOQUE_EMPRESA GetItemById(PRODUTO item);
    }
}
