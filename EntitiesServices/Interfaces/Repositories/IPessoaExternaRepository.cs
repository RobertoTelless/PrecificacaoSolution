using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPessoaExternaRepository : IRepositoryBase<PESSOA_EXTERNA>
    {
        PESSOA_EXTERNA CheckExist(PESSOA_EXTERNA item, Int32 idAss);
        PESSOA_EXTERNA GetByEmail(String email, Int32 idAss);
        PESSOA_EXTERNA GetItemById(Int32 id);
        List<PESSOA_EXTERNA> GetAllItens(Int32 idAss);
        List<PESSOA_EXTERNA> GetAllItensAdm(Int32 idAss);
        List<PESSOA_EXTERNA> ExecuteFilter(Int32? cargo, String nome, String cpf, String email, Int32 idAss);
    }
}
