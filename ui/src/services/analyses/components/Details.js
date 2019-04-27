import React from "react";
import PropTypes from 'prop-types';
import withStyles from '@material-ui/core/styles/withStyles';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import TextField from '@material-ui/core/TextField';

const styles = theme => ({
  textField: {
    marginTop: 0,
    marginLeft: theme.spacing.unit,
    marginRight: theme.spacing.unit
  }
});

function Details({ data, classes }) {
  console.log(JSON.stringify(JSON.parse(data.issues),null,2))
  return (
    <Grid container spacing={24}>
      <Grid item xs={8}>
        {data.error ?
          <TextField
            id="outlined-read-only-input"
            label="Error"
            defaultValue={data.error}
            className={classes.textField}
            multiline
            fullWidth
            InputProps={{
              readOnly: true,
            }}
            variant="outlined"
          /> : null}
        {data.issues ?
          <TextField
            id="outlined-read-only-input"
            label="Issues"
            defaultValue={JSON.stringify(JSON.parse(data.issues),null,2)}
            className={classes.textField}
            multiline
            fullWidth
            InputProps={{
              readOnly: true,
            }}
            variant="outlined"
          /> : null}
      </Grid>
      <Grid item xs={4}>
        <Typography noWrap>Api version: {data.apiVersion}</Typography>
        <Typography noWrap>Harvey version: {data.harveyVersion}</Typography>
        <Typography noWrap>Maestro version: {data.maestroVersion}</Typography>
        <Typography noWrap>Maru version: {data.maruVersion}</Typography>
        <Typography noWrap>Mythril version: {data.mythrilVersion}</Typography>
        <Typography noWrap>Submitted: {data.submittedAt}</Typography>
      </Grid>
    </Grid>
  );
}

Details.propTypes = {
  classes: PropTypes.object.isRequired,
  data: PropTypes.object.isRequired
};

export default withStyles(styles)(Details);