import { connect } from 'react-redux';
import { useEffect } from 'react';
import techChallengeApi from '../api/techChallengeApi';
import { ApplicationState } from '../store';
import * as ConfigurationStore from '../store/Configuration';

const InitialData = (props: any) => {
  const { setSearchEngines } = props;
  useEffect(() => {
    techChallengeApi
      .getSearchEngines()
      .then((response) => {
        setSearchEngines(response.data);
      })
      .catch((error) => {
        console.error(error);
      });
  }, [setSearchEngines]);

  return null;
};

export default connect(
  (state: ApplicationState) => state,
  ConfigurationStore.actionCreators
)(InitialData);
