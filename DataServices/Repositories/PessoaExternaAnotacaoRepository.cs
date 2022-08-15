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
    public class PessoaExternaAnotacaoRepository : RepositoryBase<PESSOA_EXTERNA_ANOTACAO>, IPessoaExternaAnotacaoRepository
    {
        public List<PESSOA_EXTERNA_ANOTACAO> GetAllItens()
        {
            return Db.PESSOA_EXTERNA_ANOTACAO.ToList();
        }

        public PESSOA_EXTERNA_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<PESSOA_EXTERNA_ANOTACAO> query = Db.PESSOA_EXTERNA_ANOTACAO.Where(p => p.PEAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 