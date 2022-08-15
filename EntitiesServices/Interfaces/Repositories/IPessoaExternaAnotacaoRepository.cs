using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPessoaExternaAnotacaoRepository : IRepositoryBase<PESSOA_EXTERNA_ANOTACAO>
    {
        List<PESSOA_EXTERNA_ANOTACAO> GetAllItens();
        PESSOA_EXTERNA_ANOTACAO GetItemById(Int32 id);
    }
}
