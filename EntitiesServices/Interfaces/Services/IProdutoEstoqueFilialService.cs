using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IProdutoEstoqueFilialService : IServiceBase<PRODUTO_ESTOQUE_EMPRESA>
    {
        Int32 Create(PRODUTO_ESTOQUE_EMPRESA item);
        Int32 Edit(PRODUTO_ESTOQUE_EMPRESA item);
        Int32 Delete(PRODUTO_ESTOQUE_EMPRESA item);

        List<PRODUTO_ESTOQUE_EMPRESA> GetAllItens(Int32 idAss);
        PRODUTO_ESTOQUE_EMPRESA GetByProdFilial(Int32 prod, Int32 fili);
        List<PRODUTO_ESTOQUE_EMPRESA> GetByProd(Int32 id);
        PRODUTO_ESTOQUE_EMPRESA CheckExist(PRODUTO_ESTOQUE_EMPRESA item, Int32 idAss);
        PRODUTO_ESTOQUE_EMPRESA GetItemById(Int32 id);
        PRODUTO_ESTOQUE_EMPRESA GetItemById(PRODUTO item);
    }
}
