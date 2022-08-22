using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IGrupoCCService : IServiceBase<GRUPO_PLANO_CONTA>
    {
        Int32 Create(GRUPO_PLANO_CONTA item, LOG log);
        Int32 Create(GRUPO_PLANO_CONTA item);
        Int32 Edit(GRUPO_PLANO_CONTA item, LOG log);
        Int32 Edit(GRUPO_PLANO_CONTA item);
        Int32 Delete(GRUPO_PLANO_CONTA item, LOG log);

        GRUPO_PLANO_CONTA GetItemById(Int32 id);
        GRUPO_PLANO_CONTA CheckExist(GRUPO_PLANO_CONTA item, Int32 idAss);
        List<GRUPO_PLANO_CONTA> GetAllItens(Int32 idAss);
        List<GRUPO_PLANO_CONTA> GetAllItensAdm(Int32 idAss);
    }
}
