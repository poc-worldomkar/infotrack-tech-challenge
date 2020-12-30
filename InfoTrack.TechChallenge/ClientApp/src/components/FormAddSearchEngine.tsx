import {
  Box,
  Button,
  Checkbox as MuiCheckBox,
  FormControlLabel,
  Grid,
  Paper,
  TextField as MuiTextField,
  Typography,
} from '@material-ui/core';
import { Formik, useField } from 'formik';
import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as ConfigurationStore from '../store/Configuration';
import { SearchEngine } from '../store/Configuration';

type FormAddSearchEngineProps = {
  onSubmit: (newSearchEngine: SearchEngine) => void;
};

const newSearchEngine: SearchEngine = {
  searchEngineName: '',
  searchEngineBaseUrlPath: '',
  resultXpathSelector: '',
  staticPages: false,
  parameterNameQuery: '',
  parameterNamePage: '',
  parameterNamePageSize: '',
  parameterNameRecordsSkip: '',
  dynamicPageSize: false,
  indexStartsAtOne: false,
};

const TextField = (props: any) => {
  const [field, meta] = useField(props);
  return (
    <MuiTextField
      {...field}
      {...props}
      error={meta.touched && meta.error}
      variant='outlined'
      autoCorrect='off'
      autoCapitalize='off'
      spellCheck='false'
      fullWidth
    />
  );
};

const Checkbox = ({ label, ...props }: any) => {
  const [field] = useField(props);
  return (
    <FormControlLabel
      control={<MuiCheckBox {...field} {...props} color='primary' />}
      label={label}
    />
  );
};

const FormAddSearchEngine = (props: FormAddSearchEngineProps) => {
  const { onSubmit } = props;
  return (
    <Formik initialValues={newSearchEngine} onSubmit={onSubmit}>
      {({ handleSubmit, values }: any) => {
        const { staticPages } = values;
        return (
          <form onSubmit={handleSubmit}>
            <Paper>
              <Box p={2} style={{ userSelect: 'none' }}>
                <Grid container spacing={2}>
                  <Grid item sm={12}>
                    <Typography variant='h6' component='h2'>
                      Add new Search Engine
                    </Typography>
                  </Grid>
                  <Grid item sm={4}>
                    <TextField name='searchEngineName' label='Name' required />
                  </Grid>
                  <Grid item sm={4}>
                    <TextField
                      name='searchEngineBaseUrlPath'
                      label='Base Url path'
                      required
                    />
                  </Grid>
                  <Grid item sm={4}>
                    <TextField
                      name='resultXpathSelector'
                      label='Result XPath Selector'
                      required
                    />
                  </Grid>
                  <Grid item sm={4}>
                    <TextField
                      name='parameterNameQuery'
                      label='Query parameter name'
                      required
                    />
                  </Grid>
                  <Grid item sm={4}>
                    <TextField
                      name='parameterNamePage'
                      label='Page parameter name'
                    />
                  </Grid>
                  <Grid item sm={4}>
                    <TextField
                      name='parameterNamePageSize'
                      label='Page Size parameter name'
                    />
                  </Grid>
                  <Grid item sm={4}>
                    <TextField
                      name='parameterNameRecordsSkip'
                      label='Skip Records parameter name'
                    />
                  </Grid>
                  <Grid item>
                    <Checkbox name='staticPages' label='Static search engine' />
                  </Grid>
                  <Grid item>
                    <Checkbox
                      name='dynamicPageSize'
                      label='Has flexible page results'
                    />
                  </Grid>
                  <Grid item>
                    <Checkbox
                      name='indexStartsAtOne'
                      label='Index starts at 1'
                    />
                  </Grid>
                  <Grid item xs={12}>
                    <Button
                      type='submit'
                      color='primary'
                      variant='contained'
                      disabled={staticPages}
                      fullWidth
                    >
                      Save
                    </Button>
                  </Grid>
                </Grid>
              </Box>
            </Paper>
          </form>
        );
      }}
    </Formik>
  );
};

export default connect(
  (state: ApplicationState) => state.configuration,
  ConfigurationStore.actionCreators
)(FormAddSearchEngine);
