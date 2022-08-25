using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFornecedorAnotacaoRepository : IRepositoryBase<FORNECEDOR_ANOTACOES>
    {
        List<FORNECEDOR_ANOTACOES> GetAllItens();
        FORNECEDOR_ANOTACOES GetItemById(Int32 id);
    }
}
