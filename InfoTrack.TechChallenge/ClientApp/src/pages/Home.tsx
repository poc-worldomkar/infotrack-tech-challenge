import * as React from 'react';
import { connect, shallowEqual, useSelector } from 'react-redux';
import {
  Button,
  Checkbox,
  FormControl,
  FormControlLabel,
  Grid,
  InputLabel,
  LinearProgress,
  makeStyles,
  MenuItem,
  Select,
  TextField,
  Typography,
} from '@material-ui/core';
import { useMemo, useState } from 'react';
import techChallengeApi from '../api/techChallengeApi';
import { ApplicationState } from '../store';
import * as ConfigurationStore from '../store/Configuration';
import Editor from '@monaco-editor/react';

const useStyles = makeStyles((theme) => ({
  formControl: {
    minWidth: 250,
  },
  container: {
    flex: 1,
    flexGrow: 1,
    flexShrink: 1,
  },
  editor: {
    maxHeight: '1000px',
    flex: 1,
    display: 'flex',
    maxWidth: '100%',
  },
}));

const Home = () => {
  const classes = useStyles();
  const [checkInProgress, setCheckInProgress] = useState(false);
  const [result, setResult] = useState<number[]>([]);
  const [searchEngine, setSearchEngine] = useState('');
  const [useStaticPages, setUseStaticPages] = useState(true);
  const [errorResult, setErrorResult] = useState<string | null>(null);
  const [query, setQuery] = useState('online title search');

  const searchEngines = useSelector(
    (state: any) => state.configuration.searchEngines,
    shallowEqual
  );

  const searchEngineMenuItems = useMemo(() => {
    var searchEngineMenuItems = searchEngines
      .filter((searchEngine: any) =>
        useStaticPages ? searchEngine.staticPages : !searchEngine.staticPages
      )
      .map((searchEngine: any) => (
        <MenuItem
          value={searchEngine.searchEngineName}
          key={searchEngine.searchEngineName}
        >
          {searchEngine.searchEngineName}
        </MenuItem>
      ));
    searchEngines.length > 0 &&
      !searchEngine &&
      setSearchEngine(searchEngines[0].searchEngineName);
    return searchEngineMenuItems;
  }, [searchEngines, useStaticPages, searchEngine]);

  const handleSeoCheck = () => {
    setCheckInProgress(true);
    setErrorResult(null);
    techChallengeApi
      .seoIndexCheck(searchEngine, useStaticPages, query)
      .then((response: any) => {
        setResult(response.data);
        if (response.data.length === 0) {
          setResult([0]);
        }
      })
      .catch((error) => {
        if (error.response) {
          const { data } = error.response;
          console.error(error.response);
          setErrorResult(data.error);
        }
      })
      .finally(() => {
        setCheckInProgress(false);
      });
  };

  return (
    <Grid
      container
      direction='column'
      spacing={2}
      className={classes.container}
    >
      <Grid item>
        {checkInProgress && <LinearProgress />}
        <Typography variant='h6' component='h2'>
          InfoTrack Seo Index Check
        </Typography>
      </Grid>
      <Grid item>
        <TextField
          id='query'
          label='Query'
          variant='outlined'
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          fullWidth
        />
      </Grid>
      <Grid item>
        <FormControl
          variant='outlined'
          className={classes.formControl}
          fullWidth
        >
          <InputLabel id='search-engine-label'>Search Engine</InputLabel>
          <Select
            labelId='search-engine-label'
            id='search-engine'
            value={searchEngine}
            onChange={(e) => setSearchEngine(e.target.value as string)}
            label='Search Engine'
          >
            {searchEngineMenuItems}
          </Select>
        </FormControl>
      </Grid>
      <Grid item>
        <FormControlLabel
          control={
            <Checkbox
              checked={useStaticPages}
              onChange={(e) => setUseStaticPages(!useStaticPages)}
              name='useStaticPages'
              color='primary'
            />
          }
          label='Use static pages'
        />
      </Grid>
      <Grid item>
        <Button
          variant='contained'
          color='primary'
          disabled={checkInProgress}
          onClick={handleSeoCheck}
          fullWidth
        >
          Check SEO Index
        </Button>
      </Grid>
      <Grid item>
        {result.length > 0 && (
          <Typography variant='h6' component='h2'>
            SEO Indexes: {result.join(', ')}
          </Typography>
        )}
      </Grid>
      {errorResult && (
        <>
          <Grid item>
            <Typography color='error' variant='body2'>
              Error checking Seo Index; Developer info about error below
            </Typography>
          </Grid>
          <Grid item className={classes.editor}>
            <Editor
              value={errorResult}
              theme='light'
              loading='Loading...'
              language='html'
              className={classes.editor}
            />
          </Grid>
        </>
      )}
    </Grid>
  );
};

export default connect(
  (state: ApplicationState) => state,
  ConfigurationStore.actionCreators
)(Home);
