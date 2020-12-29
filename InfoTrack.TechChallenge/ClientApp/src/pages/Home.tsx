import * as React from "react";
import { connect } from "react-redux";
import {
  Box,
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
} from "@material-ui/core";
import { useState } from "react";
import techChallengeApi from "../api/techChallengeApi";

const useStyles = makeStyles((theme) => ({
  formControl: {
    minWidth: 250,
  },
}));

const Home = () => {
  const classes = useStyles();
  const [checkInProgress, setCheckInProgress] = useState(false);
  const [result, setResult] = useState<number[]>([]);
  const [searchEngine, setSearchEngine] = useState("Bing");
  const [useStaticPages, setUseStaticPages] = useState(true);
  const [query, setQuery] = useState("online title search");

  const handleSeoCheck = () => {
    setCheckInProgress(true);
    techChallengeApi
      .seoIndexCheck(searchEngine, useStaticPages, query)
      .then((response: any) => {
        setResult(response.data);
        if (response.data.length == 0) {
          setResult([0]);
        }
        setCheckInProgress(false);
      });
  };

  return (
    <Grid container direction="column" spacing={2}>
      <Grid item>
        {checkInProgress && <LinearProgress />}
        <Typography variant="h6">InfoTrack Seo Index Check</Typography>
      </Grid>
      <Grid item>
        <TextField
          id="query"
          label="Query"
          variant="outlined"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          fullWidth
        />
      </Grid>
      <Grid item>
        <FormControl
          variant="outlined"
          className={classes.formControl}
          fullWidth
        >
          <InputLabel id="search-engine-label">Search Engine</InputLabel>
          <Select
            labelId="search-engine-label"
            id="search-engine"
            value={searchEngine}
            onChange={(e) => setSearchEngine(e.target.value as string)}
            label="Search Engine"
          >
            <MenuItem value="Bing">Bing</MenuItem>
            <MenuItem value="Google">Google</MenuItem>
          </Select>
        </FormControl>
      </Grid>
      <Grid item>
        <FormControlLabel
          control={
            <Checkbox
              checked={useStaticPages}
              onChange={(e) => setUseStaticPages(!useStaticPages)}
              name="useStaticPages"
              color="primary"
            />
          }
          label="Use static pages"
        />
      </Grid>
      <Grid item>
        <Button
          variant="contained"
          color="primary"
          disabled={checkInProgress}
          onClick={handleSeoCheck}
          fullWidth
        >
          Check SEO Index
        </Button>
      </Grid>
      <Grid item>
        {result.length > 0 && (
          <Typography variant="h6">SEO Indexes: {result.join(", ")}</Typography>
        )}
      </Grid>
    </Grid>
  );
};

export default connect()(Home);
