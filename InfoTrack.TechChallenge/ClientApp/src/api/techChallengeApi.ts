import axios from "axios";
import { SearchEngine } from "../store/Configuration";

const baseUrlTechChallenge = "techchallenge";

const getSearchEngines = () =>
  axios.get(`${baseUrlTechChallenge}/get-search-engines`);
const newSearchEngine = (searchEngine: SearchEngine) =>
  axios.post(`${baseUrlTechChallenge}/new-search-engine`, searchEngine);
const seoIndexCheck = (
  searchEngine: string,
  useStaticPages: boolean,
  query: string
) =>
  axios.post(`${baseUrlTechChallenge}/seo-index-check`, {
    searchEngine,
    useStaticPages,
    query,
  });

export default {
  getSearchEngines,
  newSearchEngine,
  seoIndexCheck,
};
