using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFormaPagRecAppService : IAppServiceBase<FORMA_PAGTO_RECTO>
    {
        Int32 ValidateCreate(FORMA_PAGTO_RECTO item, USUARIO usuario);
        Int32 ValidateEdit(FORMA_PAGTO_RECTO item, FORMA_PAGTO_RECTO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(FORMA_PAGTO_RECTO item, FORMA_PAGTO_RECTO itemAntes);
        Int32 ValidateDelete(FORMA_PAGTO_RECTO item, USUARIO usuario);
        Int32 ValidateReativar(FORMA_PAGTO_RECTO item, USUARIO usuario);

        FORMA_PAGTO_RECTO CheckExist(FORMA_PAGTO_RECTO item, Int32 idAss);
        FORMA_PAGTO_RECTO GetItemById(Int32 id);
        List<FORMA_PAGTO_RECTO> GetAllItens(Int32 idAss);
        List<FORMA_PAGTO_RECTO> GetAllItensAdm(Int32 idAss);
        Int32 ExecuteFilter(Int32? tipo, Int32? conta, String nome, Int32? idAss, out List<FORMA_PAGTO_RECTO> objeto);

        List<CONTA_BANCO> GetAllContas(Int32 idAss);
    }
}
