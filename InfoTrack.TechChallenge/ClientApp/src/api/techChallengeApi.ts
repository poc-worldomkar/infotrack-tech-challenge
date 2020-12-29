import axios from "axios";

const seoIndexCheck = (
  searchEngine: string,
  useStaticPages: boolean,
  query: string
) =>
  axios.post("techchallenge/seo-index-check", {
    searchEngine,
    useStaticPages,
    query,
  });

export default {
  seoIndexCheck,
};
