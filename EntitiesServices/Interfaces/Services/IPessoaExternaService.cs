using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPessoaExternaService : IServiceBase<PESSOA_EXTERNA>
    {
        Int32 Create(PESSOA_EXTERNA perfil, LOG log);
        Int32 Create(PESSOA_EXTERNA perfil);
        Int32 Edit(PESSOA_EXTERNA perfil, LOG log);
        Int32 Edit(PESSOA_EXTERNA perfil);
        Int32 Delete(PESSOA_EXTERNA perfil, LOG log);

        PESSOA_EXTERNA CheckExist(PESSOA_EXTERNA item, Int32 idAss);
        PESSOA_EXTERNA GetByEmail(String email, Int32 idAss);
        PESSOA_EXTERNA GetItemById(Int32 id);
        List<PESSOA_EXTERNA> GetAllItens(Int32 idAss);
        List<PESSOA_EXTERNA> GetAllItensAdm(Int32 idAss);
        List<PESSOA_EXTERNA> ExecuteFilter(Int32? cargo, String nome, String cpf, String email, Int32 idAss);

        List<CARGO_USUARIO> GetAllCargos(Int32 idAss);
        PESSOA_EXTERNA_ANEXO GetAnexoById(Int32 id);
        PESSOA_EXTERNA_ANOTACAO GetAnotacaoById(Int32 id);

    }
}
