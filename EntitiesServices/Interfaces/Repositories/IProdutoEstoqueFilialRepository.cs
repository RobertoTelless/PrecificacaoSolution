using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoEstoqueFilialRepository : IRepositoryBase<PRODUTO_ESTOQUE_EMPRESA>
    {
        List<PRODUTO_ESTOQUE_EMPRESA> GetAllItens(Int32 idAss);
        PRODUTO_ESTOQUE_EMPRESA GetByProdFilial(Int32 prod, Int32 fili);
        List<PRODUTO_ESTOQUE_EMPRESA> GetByProd(Int32 id);
        PRODUTO_ESTOQUE_EMPRESA CheckExist(PRODUTO_ESTOQUE_EMPRESA item, Int32 idAss);
        PRODUTO_ESTOQUE_EMPRESA GetItemById(Int32 id);
        PRODUTO_ESTOQUE_EMPRESA GetItemById(PRODUTO item);
    }
}
