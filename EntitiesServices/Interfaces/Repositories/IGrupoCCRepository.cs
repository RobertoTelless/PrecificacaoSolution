using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IGrupoCCRepository : IRepositoryBase<GRUPO_PLANO_CONTA>
    {
        GRUPO_PLANO_CONTA CheckExist(GRUPO_PLANO_CONTA item, Int32 idAss);
        GRUPO_PLANO_CONTA GetItemById(Int32 id);
        List<GRUPO_PLANO_CONTA> GetAllItens(Int32 idAss);
        List<GRUPO_PLANO_CONTA> GetAllItensAdm(Int32 idAss);

    }
}
