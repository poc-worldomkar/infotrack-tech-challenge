import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Chip,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  makeStyles,
  Paper,
  Typography,
} from '@material-ui/core';
import { AddCircle as AddCircleIcon } from '@material-ui/icons';
import * as React from 'react';
import { useMemo, useState } from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import techChallengeApi from '../api/techChallengeApi';
import { ApplicationState } from '../store';
import * as ConfigurationStore from '../store/Configuration';
import { SearchEngine } from '../store/Configuration';
import FormAddSearchEngine from '../components/FormAddSearchEngine';

type ConfigurationProps = ConfigurationStore.ConfigurationState &
  typeof ConfigurationStore.actionCreators &
  RouteComponentProps<{}>;

const kvpString = (key: { [key: string]: string } | null): string | null => {
  if (!key) {
    return key;
  }

  const keyName: string = Object.keys(key)[0];
  const keyValue: string = key[keyName];
  if (!keyValue) {
    return keyValue;
  }

  return `${keyName}="${keyValue}"`;
};

const useStyles = makeStyles((theme: any) => ({
  card: {
    flex: 1,
    '& .MuiCardContent-root': {
      paddingTop: 0,
      paddingBottom: theme.spacing(2),
    },
  },
}));

const Configuration = (props: ConfigurationProps) => {
  const { searchEngines } = props;
  const [showAddSearchEngineForm, setShowAddSearchEngineForm] = useState(true);
  const classes = useStyles();

  const searchEngineListItems = useMemo(() => {
    var searchEngineListItems = searchEngines.map((searchEngine: any) => {
      const {
        searchEngineName,
        searchEngineBaseUrlPath,
        resultXpathSelector,
        staticPages,
        parameterNameQuery,
        parameterNamePage,
        parameterNamePageSize,
        parameterNameRecordsSkip,
        dynamicPageSize,
        indexStartsAtOne,
      } = searchEngine;
      var searchEngineDisplayText = [
        kvpString({ searchEngineBaseUrlPath }),
        kvpString({ parameterNameQuery }),
        kvpString({ resultXpathSelector }),
        kvpString({ parameterNamePage }),
        kvpString({ parameterNamePageSize }),
        kvpString({ parameterNameRecordsSkip }),
        kvpString({ dynamicPageSize }),
        kvpString({ indexStartsAtOne }),
      ]
        .filter((s) => s)
        .join(' | ');
      return (
        <ListItem key={searchEngineDisplayText}>
          <Card className={classes.card}>
            <CardHeader
              title={searchEngineName}
              subheader={
                !staticPages && (
                  <Box mr={2} display='inline'>
                    <Chip
                      label={'Live search'}
                      variant='outlined'
                      color='primary'
                      size='small'
                    />
                  </Box>
                )
              }
            />
            <CardContent>
              <ListItemText>{searchEngineDisplayText}</ListItemText>
            </CardContent>
          </Card>
        </ListItem>
      );
    });
    return searchEngineListItems;
  }, [searchEngines]);

  const handleNewSearchEngineRequest = (newSearchEngine: SearchEngine) => {
    setShowAddSearchEngineForm(false);
    techChallengeApi
      .newSearchEngine(newSearchEngine)
      .then(() => {
        // No response on success; do trigger getsearchengines
        techChallengeApi
          .getSearchEngines()
          .then((response: any) => {
            props.setSearchEngines(response.data);
          })
          .catch((error) => {
            console.error(error);
          });
      })
      .catch((error) => {
        console.error(error);
      });
  };

  return (
    <>
      <Box display='flex' flexDirection='column' mb={2}>
        <Paper style={{ padding: 16 }}>
          <Typography variant='h6' component='h2'>
            Configuration
          </Typography>
          <List>{searchEngineListItems}</List>
          {!showAddSearchEngineForm && (
            <Button
              variant='outlined'
              color='primary'
              onClick={() =>
                setShowAddSearchEngineForm(!showAddSearchEngineForm)
              }
            >
              <ListItemIcon>
                <AddCircleIcon color='primary' />
              </ListItemIcon>
              <ListItemText primary='Add new search engine' />
            </Button>
          )}
        </Paper>
      </Box>
      {showAddSearchEngineForm && (
        <FormAddSearchEngine
          onSubmit={handleNewSearchEngineRequest}
          {...props}
        />
      )}
    </>
  );
};

export default connect(
  (state: ApplicationState) => state.configuration,
  ConfigurationStore.actionCreators
)(Configuration);
