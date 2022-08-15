using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PessoaExternaAnexoRepository : RepositoryBase<PESSOA_EXTERNA_ANEXO>, IPessoaExternaAnexoRepository
    {
        public List<PESSOA_EXTERNA_ANEXO> GetAllItens()
        {
            return Db.PESSOA_EXTERNA_ANEXO.ToList();
        }

        public PESSOA_EXTERNA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<PESSOA_EXTERNA_ANEXO> query = Db.PESSOA_EXTERNA_ANEXO.Where(p => p.PEAX_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 