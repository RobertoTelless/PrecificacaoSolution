using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFormaPagRecService : IServiceBase<FORMA_PAGTO_RECTO>
    {
        Int32 Create(FORMA_PAGTO_RECTO perfil, LOG log);
        Int32 Create(FORMA_PAGTO_RECTO perfil);
        Int32 Edit(FORMA_PAGTO_RECTO perfil, LOG log);
        Int32 Edit(FORMA_PAGTO_RECTO perfil);
        Int32 Delete(FORMA_PAGTO_RECTO perfil, LOG log);

        FORMA_PAGTO_RECTO CheckExist(FORMA_PAGTO_RECTO item, Int32 idAss);
        FORMA_PAGTO_RECTO GetItemById(Int32 id);
        List<FORMA_PAGTO_RECTO> GetAllItens(Int32 idAss);
        List<FORMA_PAGTO_RECTO> GetAllItensAdm(Int32 idAss);
        List<FORMA_PAGTO_RECTO> ExecuteFilter(Int32? tipo, Int32? conta, String nome, Int32? idAss);

        List<CONTA_BANCO> GetAllContas(Int32 idAss);
    }
}
