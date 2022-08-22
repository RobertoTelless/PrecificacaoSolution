using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ISubgrupoRepository : IRepositoryBase<SUBGRUPO_PLANO_CONTA>
    {
        SUBGRUPO_PLANO_CONTA CheckExist(SUBGRUPO_PLANO_CONTA item, Int32 idAss);
        List<SUBGRUPO_PLANO_CONTA> GetAllItens(Int32 idAss);
        SUBGRUPO_PLANO_CONTA GetItemById(Int32 id);
        List<SUBGRUPO_PLANO_CONTA> GetAllItensAdm(Int32 idAss);
    }
}
