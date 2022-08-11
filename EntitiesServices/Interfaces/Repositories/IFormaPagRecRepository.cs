using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFormaPagRecRepository : IRepositoryBase<FORMA_PAGTO_RECTO>
    {
        FORMA_PAGTO_RECTO CheckExist(FORMA_PAGTO_RECTO item, Int32 idAss);
        FORMA_PAGTO_RECTO GetItemById(Int32 id);
        List<FORMA_PAGTO_RECTO> GetAllItens(Int32 idAss);
        List<FORMA_PAGTO_RECTO> GetAllItensAdm(Int32 idAss);
        List<FORMA_PAGTO_RECTO> ExecuteFilter(Int32? tipo, Int32? conta, String nome, Int32? idAss);
    }
}

