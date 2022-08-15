using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPessoaExternaAppService : IAppServiceBase<PESSOA_EXTERNA>
    {
        Int32 ValidateCreate(PESSOA_EXTERNA item, USUARIO usuario);
        Int32 ValidateEdit(PESSOA_EXTERNA item, PESSOA_EXTERNA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(PESSOA_EXTERNA item, PESSOA_EXTERNA itemAntes);
        Int32 ValidateDelete(PESSOA_EXTERNA item, USUARIO usuario);
        Int32 ValidateReativar(PESSOA_EXTERNA item, USUARIO usuario);

        PESSOA_EXTERNA CheckExist(PESSOA_EXTERNA item, Int32 idAss);
        PESSOA_EXTERNA GetByEmail(String email, Int32 idAss);
        PESSOA_EXTERNA GetItemById(Int32 id);
        List<PESSOA_EXTERNA> GetAllItens(Int32 idAss);
        List<PESSOA_EXTERNA> GetAllItensAdm(Int32 idAss);
        Int32 ExecuteFilter(Int32? cargo, String nome, String cpf, String email, Int32 idAss, out List<PESSOA_EXTERNA> objeto);

        List<CARGO_USUARIO> GetAllCargos(Int32 idAss);
    }
}
