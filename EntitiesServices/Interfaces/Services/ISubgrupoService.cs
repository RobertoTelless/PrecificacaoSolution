using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ISubgrupoService : IServiceBase<SUBGRUPO_PLANO_CONTA>
    {
        Int32 Create(SUBGRUPO_PLANO_CONTA item, LOG log);
        Int32 Create(SUBGRUPO_PLANO_CONTA item);
        Int32 Edit(SUBGRUPO_PLANO_CONTA item, LOG log);
        Int32 Edit(SUBGRUPO_PLANO_CONTA item);
        Int32 Delete(SUBGRUPO_PLANO_CONTA item, LOG log);

        SUBGRUPO_PLANO_CONTA GetItemById(Int32 id);
        SUBGRUPO_PLANO_CONTA CheckExist(SUBGRUPO_PLANO_CONTA item, Int32 idAss);
        List<SUBGRUPO_PLANO_CONTA> GetAllItens(Int32 idAss);
        List<SUBGRUPO_PLANO_CONTA> GetAllItensAdm(Int32 idAss);
        List<GRUPO_PLANO_CONTA> GetAllGrupos(Int32 idAss);
    }
}
