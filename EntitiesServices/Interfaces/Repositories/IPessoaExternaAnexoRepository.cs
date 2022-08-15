using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPessoaExternaAnexoRepository : IRepositoryBase<PESSOA_EXTERNA_ANEXO>
    {
        List<PESSOA_EXTERNA_ANEXO> GetAllItens();
        PESSOA_EXTERNA_ANEXO GetItemById(Int32 id);
    }
}
