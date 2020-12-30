import * as React from "react";
import { connect, shallowEqual, useSelector } from "react-redux";
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
import { useEffect } from "react";
import techChallengeApi from "../api/techChallengeApi";
import { ApplicationState } from "../store";
import * as ConfigurationStore from "../store/Configuration";

const InitialData = (props: any) => {
  useEffect(() => {
    techChallengeApi
      .getSearchEngines()
      .then((response) => {
        props.setSearchEngines(response.data);
      })
      .catch((error) => {
        console.error(error);
      });
  }, []);

  return null;
};

export default connect(
  (state: ApplicationState) => state,
  ConfigurationStore.actionCreators
)(InitialData);
